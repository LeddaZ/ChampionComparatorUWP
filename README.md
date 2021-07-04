# ChampionComparatorUWP
UWP version of [ChampionComparatorGUI](https://github.com/LeddaZ/ChampionComparatorGUI). Requires Windows 10 1709 or newer.

## Instructions
You can download the latest release from [here](https://github.com/LeddaZ/ChampionComparatorUWP/releases/latest). Since this app is self-signed, you'll need to install [this](https://github.com/LeddaZ/LeddaZ.github.io/raw/master/files/certificate.cer) certificate before installing the app.

- Double click on `certificate.cer` and install it to local machine.
- Choose `Place all certificates in the following store` > `Browse` > `Trusted Root Certification Authorities`
- After installing the certificate, double-click on the `.msixbundle` file to install it.

## Other software
- All JSON-related stuff (parsing stats, game patch, etc.) is implemented using [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json).
- The update checking code, which uses GitHub's API to get necessary data from the releases, is implemented using [Octokit](https://github.com/octokit/octokit.net).
