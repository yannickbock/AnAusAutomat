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
| Name | Type | Default Value | Description |
| ------ | ------ | ------ | ------ |
| | | | |

### Socket
| Name | Type | Default Value | Description |
| ------ | ------ | ------ | ------ |
| OffDelaySeconds | uint ( x > 0 ) | 300 | Time between last interaction and PowerOff. |

## Features
- SendStatusForecast
