#!/bin/sh
echo -ne '\033c\033]0;BetterCalibratorPopup\a'
base_path="$(dirname "$(realpath "$0")")"
"$base_path/calibrator.x86_64" "$@"
