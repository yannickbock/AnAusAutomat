# SoundSniffer
Monitors sound peak level.

## Status

#### PowerOn
After x seconds music playback if volume is not muted or not zero. (MinimumSignalSeconds)

#### PowerOff
After x seconds without any interaction. (OffDelaySeconds)

#### Undefined
Not supported.

## Parameters

### Global
| Name | Values | Default | Multiple | Example | Description |
| ------ | ------ | ------ | ------ | ------ | ------ |
| | | | |

### Socket
| Name | Values | Default | Multiple | Example | Description |
| ------ | ------ | ------ | ------ | ------ | ------ |
| MinimumSignalSeconds | 0 to 4294967295 | 3 | no | | Minimum duration music playback. |
| OffDelaySeconds | 0 to 4294967295 | 300 | no | | Period between last interaction and PowerOff. |

## Features
- SendStatusForecast
