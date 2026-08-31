using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Unity2DTemplate.Features
{
    internal static class ProfileStorage
    {
        private const string FileName = "profile.json";
        private const string PlayerPrefsBackupKey = "Unity2DTemplate.Profile.Backup";
        private static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false);

        public static ProfileData Load()
        {
            ProfileData best = null;

#if !UNITY_WEBGL || UNITY_EDITOR
            string path = GetPath();
            ConsiderCandidate(TryReadFile(path), ref best);
            ConsiderCandidate(TryReadFile(path + ".bak"), ref best);
            ConsiderCandidate(TryReadFile(path + ".tmp"), ref best);
#endif

            if (PlayerPrefs.HasKey(PlayerPrefsBackupKey))
            {
                ConsiderCandidate(
                    TryParse(PlayerPrefs.GetString(PlayerPrefsBackupKey)),
                    ref best);
            }

            return best ?? new ProfileData();
        }

        public static bool Save(ProfileData data)
        {
            if (data == null)
            {
                return false;
            }

            data.version = GameProfile.CurrentSaveVersion;
            data.revision = data.revision < long.MaxValue
                ? data.revision + 1L
                : long.MaxValue;
            data.integrityMarker = GameProfile.SaveIntegrityMarker;

            string json = JsonUtility.ToJson(data, true);
            PlayerPrefs.SetString(PlayerPrefsBackupKey, json);
            PlayerPrefs.Save();

#if UNITY_WEBGL && !UNITY_EDITOR
            return true;
#else
            return TryWriteFile(GetPath(), json);
#endif
        }

        private static ProfileData TryReadFile(string path)
        {
            try
            {
                return File.Exists(path)
                    ? TryParse(File.ReadAllText(path, Utf8WithoutBom))
                    : null;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Could not read profile candidate '{path}': {exception.Message}");
                return null;
            }
        }

        private static ProfileData TryParse(string json)
        {
            if (string.IsNullOrWhiteSpace(json)
                || !json.TrimEnd().EndsWith("}", StringComparison.Ordinal))
            {
                return null;
            }

            try
            {
                ProfileData data = JsonUtility.FromJson<ProfileData>(json);
                if (data == null
                    || data.version != GameProfile.CurrentSaveVersion
                    || data.revision < 0L
                    || data.integrityMarker != GameProfile.SaveIntegrityMarker)
                {
                    return null;
                }

                data.statistics ??= new System.Collections.Generic.List<StatisticRecord>();
                data.achievements ??= new System.Collections.Generic.List<UnlockRecord>();
                data.galleryEntries ??= new System.Collections.Generic.List<UnlockRecord>();
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void ConsiderCandidate(ProfileData candidate, ref ProfileData best)
        {
            if (candidate != null && (best == null || candidate.revision > best.revision))
            {
                best = candidate;
            }
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        private static bool TryWriteFile(string path, string json)
        {
            string temporaryPath = path + ".tmp";
            string backupPath = path + ".bak";

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                byte[] bytes = Utf8WithoutBom.GetBytes(json);
                using (FileStream stream = new FileStream(
                           temporaryPath,
                           FileMode.Create,
                           FileAccess.Write,
                           FileShare.None))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush(true);
                }

                if (File.Exists(path))
                {
                    File.Copy(path, backupPath, true);
                }

                if (File.Exists(path))
                {
                    try
                    {
                        File.Replace(temporaryPath, path, null, true);
                    }
                    catch (PlatformNotSupportedException)
                    {
                        File.Copy(temporaryPath, path, true);
                        File.Delete(temporaryPath);
                    }
                    catch (IOException)
                    {
                        File.Copy(temporaryPath, path, true);
                        File.Delete(temporaryPath);
                    }
                }
                else
                {
                    File.Move(temporaryPath, path);
                }

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Could not save profile '{path}': {exception}");
                return false;
            }
        }

        private static string GetPath()
        {
            return Path.Combine(Application.persistentDataPath, FileName);
        }
#endif
    }
}
