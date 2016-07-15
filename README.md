
# Dropbox PowerShell Module

A powershell module used to mount Dropbox accounts through a provider.

# Example Usage

```powershell
import-module dropbox
new-psdrive dbox -psprovider dropbox -root ""
## at this point powershell will ask you to authenticate with dropbox
cd dbox:\
dir
```

# Supported Operations

Check the about_Dropbox_Version help topic for specific feature availability.
