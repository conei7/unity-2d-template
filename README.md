# Unity 2D Template

Unity 2Dゲームをすばやく作り始めるための個人用テンプレートです。
基本的なシーン遷移、音量設定、日本語フォント、確認ダイアログ、
クレジット、統計、実績、ギャラリーをあらかじめ用意しています。

## 動作環境

- Unity `6000.5.5f1`
- Universal Render Pipeline（2D Renderer）
- Input System
- TextMesh Pro
- [MCP for Unity](https://github.com/CoplayDev/unity-mcp) `v10.1.2`

## 新しいゲームを始める

1. このリポジトリを複製またはTemplate Repositoryから作成します。
2. Unity Hubでリポジトリ直下を開きます。
3. [Title.unity](Assets/_Project/Scenes/Title.unity)を開き、Playして一通りの画面を確認します。
4. Player SettingsのCompany Name、Product Name、Package Nameを変更します。
5. `Title`、`Game`、`Result`の仮テキストとゲーム部分を作品用に置き換えます。
6. 不要な任意機能は、後述の「機能を外す」を参考に削除します。

`Library`、`Temp`、`Logs`、`UserSettings`などの生成フォルダは
Gitで管理しません。

### 複製時のチェックリスト

- ゲームごとに別のリポジトリを作り、このテンプレートの`main`または安定版タグから開始する
- Unityを閉じた状態で複製し、`Library`、`Temp`、`Logs`、`UserSettings`を持ち込まない
- リポジトリのフォルダ名を作品ごとに一意にする
- Player SettingsのCompany Name、Product Name、Package Nameを作品用に変更する
- READMEのタイトルと概要を作品用に変更する
- 最初のゲーム実装前にTest RunnerとTitleシーンのPlay確認を行う

複製後は元テンプレートの`origin`へ誤ってpushしないよう、
新しいゲーム用リポジトリを`origin`に設定してから作業を始めます。

## フォルダ構成

```text
Assets/
├─ TextMesh Pro/       # TMP Essential Resources
└─ _Project/
   ├─ Art/             # Sprite、Animationなど
   ├─ Audio/           # BGM、SE、AudioMixer
   ├─ Data/            # ScriptableObjectのゲームデータ
   ├─ Prefabs/         # 共通Prefab
   ├─ Resources/       # Noto Sans JPなどの共有アセット
   ├─ Scenes/          # 基本フローと任意機能のScene
   ├─ Scripts/
   │  ├─ Runtime/      # ゲーム実行時のコード
   │  └─ Editor/       # Editor専用コード
   ├─ Settings/        # URP、Input System設定
   └─ Tests/
```

ゲーム固有のアセットも、原則として `Assets/_Project/` 以下へ追加します。

## シーン構成

```text
Title ──> Game ──> Result ──> Title
  │
  └──> Extras
         ├──> Credits
         ├──> Statistics
         ├──> Achievements
         └──> Gallery
```

- `Title`：ゲーム開始、任意機能、終了
- `Game`：ゲーム本編の差し替え先
- `Result`：リザルト画面の差し替え先
- `Extras`：任意機能へ移動するハブ
- `Credits`：クレジット
- `Statistics`：統計
- `Achievements`：実績
- `Gallery`：画像・音声ギャラリー

遷移先はBuild Settingsの番号ではなく、
`SceneLoader.LoadScene(string)`へシーン名を明示して指定します。
新しいシーンを追加した場合は、Build Settingsにも忘れず追加してください。

## 新しいシーンを作る

共通機能が必要なシーンには、次のPrefabを追加します。

- `AudioManager.prefab`
- `GameProfile.prefab`
- `SettingsMenu.prefab`

さらにuGUIを使うシーンには、`EventSystem`と
`InputSystemUIInputModule`を配置します。
`AudioManager`と`GameProfile`はシーンをまたいで残り、
重複したインスタンスは自身を破棄します。

## シーン遷移

遷移ボタンのOn Clickへ、シーン内の`SceneLoader`と
`LoadScene(string)`を登録し、引数に遷移先のシーン名を入力します。

```csharp
sceneLoader.LoadScene("Game");
```

Titleの終了ボタンは確認後に`Application.Quit()`を呼びます。
Unity Editorでは終了せず、ビルドしたゲームで動作します。

## 設定画面と音声

どのシーンでも`Esc`で設定画面を開閉できます。
設定画面を開いている間は`Time.timeScale`が0になります。

`AudioManager`はBGMとSEの再生、AudioMixerの音量、音量設定の保存を担当します。

```csharp
AudioManager.Instance.PlayBgm(bgmClip);
AudioManager.Instance.StopBgm();
AudioManager.Instance.PlaySe(seClip);
```

音量は`MainAudioMixer`の`BGM`・`SE`グループへ反映され、
`PlayerPrefs`へ保存されます。

## はい／いいえ確認ダイアログ

`ConfirmationDialog.prefab`は、タイトル、本文、「はい」「いいえ」を持つ
共通Prefabです。使用するシーンのCanvasへ配置して参照を渡します。

```csharp
confirmationDialog.Show(
    "セーブデータを削除しますか？",
    "この操作は取り消せません。",
    DeleteSaveData);
```

- コールバックは一度だけ実行されます。
- 初期選択は安全側の「いいえ」です。
- `SettingsMenu.prefab`にはあらかじめ組み込み済みです。
- Settings Menuから表示した場合は、`Esc`でもキャンセルできます。

## 統計・実績・ギャラリーの保存

`GameProfile`が、シーンをまたぐ統計値と解除状態を管理します。

```csharp
GameProfile.Instance.AddStatistic("enemies_defeated", 1);
GameProfile.Instance.SetHighestStatistic("high_score", score);
GameProfile.Instance.SetLowestPositiveStatistic("best_time_seconds", clearTime);
GameProfile.Instance.UnlockAchievement("first_clear");
GameProfile.Instance.UnlockGalleryEntry("ending_art");
```

データは`Application.persistentDataPath`へバージョン付きJSONとして保存します。
一時ファイルとバックアップを使用し、WebGLではPlayerPrefsバックアップを
主な保存先として使用します。

### 統計を追加する

1. `Assets/_Project/Data/Features/StatisticsCatalog.asset`を開きます。
2. 一意で変更しない文字列ID、表示名、表示形式を追加します。
3. ゲーム中の必要なタイミングで`GameProfile`の記録APIを呼びます。
4. 表示件数が既存の行数を超える場合は、`Statistics`シーンでRowを複製し、
   `StatisticsPanelController`のRows配列へ登録します。

表示形式は整数、小数、時間から選択できます。

### 実績を追加する

1. `Assets/_Project/Data/Features/AchievementCatalog.asset`を開きます。
2. 一意で変更しない文字列ID、タイトル、説明、アイコンを追加します。
3. 達成条件を満たした場所で`UnlockAchievement(id)`を呼びます。
4. 表示件数が増えた場合は、`Achievements`シーンでRowを複製して配列へ登録します。

実績解除時は`SettingsMenu.prefab`内のトーストが表示されます。

### ギャラリーを追加する

1. `Assets/_Project/Data/Features/GalleryCatalog.asset`を開きます。
2. ID、タイトル、制作者、説明、画像、音声を設定します。
3. 最初から表示する項目は`Unlocked By Default`を有効にします。
4. 条件付き項目は`UnlockGalleryEntry(id)`で解除します。
5. 項目数が増えた場合は、`Gallery`シーンでSlotを複製して配列へ登録します。

画像表示と音声再生は同じ項目に設定できます。ギャラリー音声はSEの
AudioMixerグループへ出力されます。

## クレジットを編集する

`Assets/_Project/Data/Features/CreditsCatalog.asset`で、
セクション名と本文を編集します。フォントのライセンス表記を消す場合は、
使用しているフォントのライセンス条件を先に確認してください。

## UIを増やすとき

統計行、実績行、ギャラリーカードを増やす場合は、
Unity Editor上で既存のRowまたはSlotを複製し、
Controllerのシリアライズ配列へ追加してください。

## 機能を外す

任意機能を使用しない場合は、次の順番で外します。

1. 統計を使わない場合は、`Game`シーンの`Game Session Statistics`と、
   それを呼ぶButtonの永続On Clickリスナーをセットで削除します。
2. `Extras`シーンから対象機能のボタンを削除します。
3. 対象の機能シーンをBuild Settingsから外します。
4. 対象シーンとCatalogを削除します。
5. 他から参照されていなければ、対応するControllerスクリプトを削除します。

統計・実績・ギャラリーのいずれかを使用する場合は、
`GameProfile`と保存関連スクリプトを残してください。

## 日本語フォント

`NotoSansJP-Dynamic`をTextMesh Proのデフォルトフォントに設定しています。
Dynamic・Multi Atlas構成のため、日本語全文字のSDF Atlasを
あらかじめ保持せず、必要な文字だけを実行時に生成します。

EditorでPlayしたときに生成された文字Atlasは、Play終了後に自動で消去します。
これにより、動的キャッシュによる巨大なGit差分が残るのを防ぎます。
必要なら`Tools > Unity 2D Template > Clear Dynamic Font Data`から手動消去できます。

フォントはSIL Open Font Licenseです。ライセンスは
`Assets/_Project/Resources/Fonts/NotoSansJP/OFL.txt`を参照してください。

## 自動テスト

`Assets/_Project/Tests/`にテンプレートのスモークテストがあります。

- EditMode：Build Settingsの基本シーンと、シーン内のMissing Scriptを確認する
- PlayMode：Build Settingsで有効な全シーンを実際に順番にロードする

Unityの`Window > General > Test Runner`からEditMode、PlayModeを実行できます。
テンプレートを複製した直後と、共通シーンやPrefabを変更した後に実行してください。
