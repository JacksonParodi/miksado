# miksado
### a Twitch-connected tool for BizHawk

***

currently available plugin:

- ### game shuffler
automatically swap the current game with a random one.

game shuffles can be triggered by:
  - timer
  - Twitch subs
  - Twitch bits
  - Twitch channel point redemptions
  - Twitch chat command

---

## instructions

1. [download the latest release zip](https://github.com/JacksonParodi/miksado/releases)
2. highly recommended to use a fresh install of [BizHawk 2.11](https://github.com/TASEmulators/BizHawk/releases/tag/2.11)
3. unzip contents into BizHawk base directory
4. run MiksadoInstaller.exe
5. place game files in the `miksado/game` folder from BizHawk base directory
6. start using miksado
    - run the created `run_miksado.bat` script in the BizHawk base directory
    - in EmuHawk, in Tools > External Tools > miksado
7. click `authorize` button and approve in web browser for Twitch capabilities
8. with a new install of BizHawk, you can now set your controls and any other emulator settings

---

## usage

### game shuffler

- `new` button starts a brand new session, deleting any prior save state files and completion flags
- `resume` button will pick up a previous session, loading in any previous save states and completion flags
- `pause` and `unpause` only stop/start the current active session. when paused, no shuffle triggers will be active
- the `cooldown` control is the number of seconds guaranteed before the next possible game shuffle
- for the channel point redemption, simply create a redemption in your Twitch dashboard with the same exact name as the one typed in the redemption tab panel
- the poll tab does nothing right now, but it will in future versions...
