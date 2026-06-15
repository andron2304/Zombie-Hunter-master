Як налаштувати кнопку "Settings" і регулювачі гучності

Коротко: додайте панель налаштувань у сцену, прикріпіть компонент `SettingsMenu`, створіть три `Slider` для Master/Music/SFX і підключіть їх у Інспекторі.

Кроки:

1. UI: у вашому `Canvas` створіть `Panel` → назвімо її `SettingsPanel`.
2. Додайте всередину `SettingsPanel` три `Slider` (й підписи): `MasterSlider`, `MusicSlider`, `SFXSlider`.
   - Задайте `Min=0` і `Max=1` для кожного слайдера.
3. Створіть порожній GameObject у сцені (наприклад `SettingsManager`) або використайте `SettingsPanel`.
4. Додайте компонент `SettingsMenu` (файл: Assets/Scripts/SettingsMenu.cs) на `SettingsManager`.
5. У Інспекторі перетягніть `MasterSlider`, `MusicSlider`, `SFXSlider` у відповідні поля скрипта.

Підключення AudioMixer (рекомендовано):

- Створіть `AudioMixer` (Window → Audio → Audio Mixer). Умістіть у ньому групи і витExpose параметри гучності, наприклад `MasterVolume`, `MusicVolume`, `SFXVolume`.
- Перетягніть AudioMixer в поле `audioMixer` компонента `SettingsMenu`.
- У `SettingsMenu` скрипті значення слайдерів перетворюються у децибели (0..1 → -80..0 dB) і задаються в AudioMixer.

Якщо у вас немає AudioMixer:

- Скрипт використовує `AudioListener.volume` як fallback для `Master`.
- Для `Music` можна прив'язати `AudioSource` (перетягніть у поле `musicSource`) — тоді слайдер регулюватиме саме його гучність.

Підключення подій UI:

- Для кожного `Slider` у `On Value Changed (Single)` додайте виклик відповідного метода з `SettingsMenu`:
  - `MasterSlider` → `SettingsMenu.SetMasterVolume (float)`
  - `MusicSlider` → `SettingsMenu.SetMusicVolume (float)`
  - `SFXSlider` → `SettingsMenu.SetSFXVolume (float)`

Кнопка "Settings":

- Додайте кнопку у головному меню і в `OnClick()` вкажіть `SettingsMenu.TogglePanel` або `ShowPanel` з передачею `SettingsPanel` (drag the panel GameObject as argument).

Збереження:

- Скрипт автоматично зберігає значення у `PlayerPrefs` під ключами `masterVolume`, `musicVolume`, `sfxVolume`.

Додатково:

- Якщо SFX у вашій грі керуються різними `AudioSource`, можна при відтворенні читати `PlayerPrefs.GetFloat("sfxVolume",1f)` і множити гучність.
- Для тестування: запустіть сцену в Editor, відкрийте `SettingsPanel` і тягніть слайдери — зміни повинні застосовуватись одразу.
