# CrmOpenFormSubmitService

## Overview

Monitors a folder for Created *.txt files. Reads the file and submits it to CrmOpen Handler Page.

## Installation

1. Open command line as Administrator
2. CD to CrmOpenFormSubmitService folder
3. > install.cmd
4. Edit CrmOpenFormSubmitService.exe.config and set watchFolder to your folder.
5. > Icacls C:\shared /grant Everyone:F /inheritance:e /T     <--- Replace c:\shared with your folder and run
6. Open Services and Start CrmOpenFormSubmitService
7. Confirm that log.txt is created in /logs and that there are no errors.

