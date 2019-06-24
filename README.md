# WindowsUpdateX
## Better windows updates
Made for my AQA Computer Science A-Level project 2018

### Purpose
* Allow a power user to customise how they install windows updates
* Make windows updates less intrusive by not forcing them on you until you ask for them
* Installs updates in batches to make troubleshooting simpler

### Features
* Provides a UI for viewing queued updates
* Allows batch install of updates according to user preference
* Uses system performance metrics to judge if an update has caused system issues

### Installation
###### Windows Installer (.msi)
    * Download the latest release and run the installer
###### Manual Install
    * Clone this repo and build the installer with visual studio etc.

### Dependencies
* Microsoft SQL Server (installed automatically by the msi)
### Disclaimer
This program gathers performance metrics and other data about your system specs but they are stored locally only. I as the developer have no access to this data unless you choose to share it with me.