# Adder AIM Manager API
This example C# code allows you to control and interact with the Adder AIM Manager product.

https://www.adder.com/en/kvm-solutions/adderlink-infinity-manager-v4

IMPORTANT: The code is provide as is, with no support or warranty.

It provides the following functions:
-Observable collections for Devices, Channels, Presets and C-USBLAN's.
-Asynchronous updates to the collections.
-Event driven changes.
-Contains all the functions supported by the AIM's API.

For the asynchronous updates to work, there is a function called GetAsync that takes a SynchronizationContext parameter that is used to switch between the working and main threads when working with the events.
You must provide an IP Address, Username and Password.
The API is restful, to get or update the current configurations, you can either use GetData or GetDataAsync. GetDataAsync will run on another thread.
If AutoUpdate is enabled, calling GetDataAsync will automatically fetch an update every 5 seconds.
Any updates to the Observerable collections trigger corresponding events.

For further information about the RedPSU API, please visit: http://support.adder.com/tiki/tiki-index.php?page=ALIF%3A%20API
