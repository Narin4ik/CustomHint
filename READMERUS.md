# CustomHint
## Описание
Плагин позволяющий сделать свой собственный HUD для сервера.  
Для нормальной работы плагина потребуется использовать **HintServiceMeow** и **Newtonsoft.Json**, который есть в релизах на всех версиях.  
Для предложений, пожалуйста, пингуйте в Discord на сервере EXILED либо пишите в лс: @narin4ik  
## Обучение
### Как устанновить плагин?
Переходим на [последний релиз.](https://github.com/BTF-SCPSL/CustomHint/releases) После чего скачиваем все *dll* файлы из релиза, после чего загружаем на сервер: *CustomHint.dll* и *HintServiceMeow.dll* в папку Plugins (`.../EXILED/Plugins`), а *Newtonsoft.Json.dll* в папку dependencies (`.../EXILED/Plugins/dependencies`).  
После установки *запускаем/перезапускаем* сервер.  
Как только Вы сделали все манипуляции, в `.../EXILED/Configs` в файлах `[port]-config.yml` и `[port]-translation.yml` сгенерируется конфигурация.
### Настройка плагина
Начнём с самого лёгкого, а именно с `[port]-config.yml`. Используя комбинацию *CTRL+F* и вписывая `custom_hint`, находим конфигурацию плагина.  
Дефолтная конфигурация `[port]-config.yml` выглядит вот так, там рассписаны все пункты:
```yaml
custom_hint:
# Plugin enabled (bool)?
  is_enabled: true
  # Debug mode?
  debug: false
  # Enable or disable HUD-related commands.
  enable_hud_commands: true
  # Enable or disable automatic plugin updates.
  auto_updater: true
  # The interval for changing {hints} placeholder (in seconds).
  hint_message_time: 5
  # Default role name for players without a role.
  default_role_name: 'Player'
  # Default role color (for players without roles).
  default_role_color: 'white'
  # Server timezone for placeholder. Use 'UTC' by default or a valid timezone ID (e.g., 'Europe/Kyiv').
  server_time_zone: 'UTC'
  # Enable counting Overwatch players in placeholder {spectators_num}.
  enable_overwatch_counting: true
  # List of hints.
  hints:
  - id: 'firsthint'
    text: 'Hello World!'
    font_size: 15
    position_x: 500
    position_y: 500
    can_be_hidden: true
    roles:
    - ClassD
```
После того как настроили `[port]-config.yml`, переходим в `[port]-translation.yml`, снова используем комбинацию *CTRL+F* и вписываем `custom_hint`.  
Там увидим следующее:
```yaml
custom_hint:
# Message displayed when the HUD is successfully hidden.
  hide_hud_success_message: '<color=green>You have successfully hidden the server HUD! To get the HUD back, use .showhud.</color>'
  # Message displayed when HUD is already hidden.
  hide_hud_already_hidden_message: '<color=red>You''ve already hidden the server HUD.</color>'
  # Message displayed when HUD is successfully shown.
  show_hud_success_message: '<color=green>You have successfully returned the server HUD! To hide again, use .hidehud</color>'
  # Message displayed when HUD is already shown.
  show_hud_already_shown_message: '<color=red>You already have the server HUD displayed.</color>'
  # Message displayed when DNT (Do Not Track) mode is enabled.
  dnt_enabled_message: '<color=red>Disable DNT (Do Not Track) mode.</color>'
  # Message displayed when commands are disabled on the server.
  command_disabled_message: '<color=red>This command is disabled on the server.</color>'
  # Round time.
  round_time_formats:
    seconds: '{round_duration_seconds} seconds'
    minutes: '{round_duration_minutes} minutes {round_duration_seconds} seconds'
    hours: '{round_duration_hours} hours {round_duration_minutes} minutes {round_duration_seconds} seconds'
  # Game Role of a player, {player_gamerole} is placeholder.
  game_roles:
  - role: Tutorial
    name: 'Tutorial'
  - role: ClassD
    name: 'Class-D'
  - role: Scientist
    name: 'Scientist'
  - role: FacilityGuard
    name: 'Facility Guard'
  - role: Filmmaker
    name: 'Film Maker'
  - role: Overwatch
    name: 'Overwatch'
  - role: NtfPrivate
    name: 'MTF Private'
  - role: NtfSergeant
    name: 'MTF Sergeant'
  - role: NtfSpecialist
    name: 'MTF Specialist'
  - role: NtfCaptain
    name: 'MTF Captain'
  - role: ChaosConscript
    name: 'CI Conscript'
  - role: ChaosRifleman
    name: 'CI Rifleman'
  - role: ChaosRepressor
    name: 'CI Repressor'
  - role: ChaosMarauder
    name: 'CI Marauder'
  - role: Scp049
    name: 'SCP-049'
  - role: Scp0492
    name: 'SCP-049-2'
  - role: Scp079
    name: 'SCP-079'
  - role: Scp096
    name: 'SCP-096'
  - role: Scp106
    name: 'SCP-106'
  - role: Scp173
    name: 'SCP-173'
  - role: Scp939
    name: 'SCP-939'
```
И после локализации... Voilà! Всё готово! Можете рестартнуть *(полнотью)* сервер и CustomHint работает на отлично.  
Спасибо тем, кто использует данный плагин. Удачи Вам!  
## Placeholders
| Placeholder       | Описание                                 |
| ----------------- | ---------------------------------------- |
| {servername}      | Название сервера.                            |
| {ip}              | IP-адрес сервера.                       |
| {port}            | Порт сервера.                           |
| {tps}             | TPS сервера.                            |
| {player_nickname} | Никнейм игрока.                         |
| {player_role}     | Роль игрока.                            |
| {player_gamerole}       | Игровая роль игрока.                |
| {round_time}| Общая продолжительность раунда.                 |
| {round_duration_seconds} | Длительность раунда в секундах.    |
| {round_duration_minutes} | Длительность раунда в минутах.    |
| {round_duration_hours}   | Длительность раунда в часах.      |
| {classd_num}   | Количество Класс-D персонала.      |
| {scientist_num}   | Количество Научных Сотрудников.      |
| {facilityguard_num}   | Количество Охранников Комплекса.      |
| {mtf_num}   | Количество МОГ.      |
| {ci_num}   | Количество ПХ.      |
| {scp_num}   | Количество SCP объектов.      |
| {spectators_num}   | Количество наблюдателей (включая Overwatch).      |
| {generators_activated}   | Количество активируемых генераторов.      |
| {generators_max}   | Максимальное количество генераторов.      |
| {current_time}   | Текущее реальное время по-вашему часовому поясу.      |
| {hints}           | Hints из файла Hints.txt.  |

## Сторонние placeholders
| Автор            | Название            | Описание                                 |
| ----------------------- | ----------------------- | ------------------------------------------ |
| [Narin](https://github.com/Narin4ik)            | [SteamIDPlaceholder](https://github.com/Narin4ik/SteamIDPlaceholder) | Отображает STEAMID64 в виде плейсхолдера. |
| [Chuppa2](https://github.com/Chuppa2)            | [CRCHPlaceholder](https://github.com/Chuppa2/CRCHPlaceholder)                               | Отображает пользовательскую информацию наблюдаемого игрока.                               |
| [Narin](https://github.com/Narin4ik)            | [RespawnTimer](https://github.com/Narin4ik/RespawnTimer)                               | Добавляет таймера до прибытия МОГ и ПХ.                               |
