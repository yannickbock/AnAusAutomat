# Keylogger
Listens to mouse move and key pressed event.

## Status
#### PowerOn
Immediately after a key is pressed or the mouse cursor was moved.
#### PowerOff
After x seconds without any interaction.
#### Undefined
Not supported.

## Parameters

### Global
| Name | Valid values | Default value | Description |
| ------ | ------ | ------ | ------ |
| | | | |

### Socket
| Name | Valid values | Default value | Description |
| ------ | ------ | ------ | ------ |
| OffDelaySeconds | 0 to 4294967295 | 300 | Period between last interaction and PowerOff. |

## Features
- SendStatusForecast
