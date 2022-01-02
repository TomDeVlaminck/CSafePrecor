# CSafePrecor
c# library to retrieve fitness data from a Precor Crosstrainer  EFX 546i via the CSAFE port and a SER2NET espeasy ESP82666 module

I have made the following setup to communicate wit the Precor Crosstrainer (but this should work for all CSAFE ports)

RJ45 connector (looking inside the rj45 connector - T-569B cabling)

---------------
1/2/3/4/5/6/7/8
---------------
   (NOTCH)

1 - orange+white
2 - orange
3 - green+white <= RX
4 - blue.       => TX
5 - blue+white  => VOUT
6 - green
7 - brown-white => GND
8 - brown

RX connected to output of RS232 <-> TTL shifter
TX connected to input of RS232 <-> TTL shifter
GND connected to GND of RS232 <-> TTL shifter

VOUT connected to input of low drop voltage converter 3.3V output
GND connected to GND of low drop voltage converter GND
3.3V output of low drop voltage converter connected tot 3.3V line of ESP8266 D1 Mini V2 | WiFi | CH340G Development board (Wemos)
GND output of low drop voltage converter connected tot GND line of ESP8266 D1 Mini V2 | WiFi | CH340G Development board (Wemos)
D10 (GPIO-1) TX line of ESP8266 D1 Mini V2 | WiFi | CH340G Development board (Wemos) connected to RX input of RS232 <-> TTL shifter
D9 (GPIO-3) RX line of ESP8266 D1 Mini V2 | WiFi | CH340G Development board (Wemos) connected to TX output of RS232 <-> TTL shifter

See schematic and pictures for more info (TODO)

Next the ESP8266 was flashed with espeasy
- Configure WIFI access and WIFI backup
- Setup a name
- Add device "Communication - Serial Server"
- Serial port : HW Serial 0
- TCP port : 1234
- Baud Rate : 9600
- Serial Config : 8 bit / parity:None / Stop bits:1
- Event processing none
- RX timeout : 20 ms
- RX buffer size : 256
- Enabled : Checked

Next you should be able to verify if the SER2NET bridge is working by starting the fitness device, it will send some binary data.
Using the supplied python script one can see the data in hex (TODO)

Next you need to download the repository and open the project in visual studio (mac)
By changing the name and port in the devices (todo change this) one should be able to retrieve some basic information from the device.

As this is a W.I.P. i hope this has some use to someone (input is always welcomed).
Credits must also go to https://github.com/b0urb4k1/Concept2-Rower as I based my code on his code. But this is for a Concept2 rowing device that communicates via usb. My Precor is via old school RS232 :)

My final goal is to make an API that can be used by Openhab or other domotics system so that I can :
- Upload my training program
- See my workout in grafana (the moment when i get a heart attack)
- See my progress
- Start a workout with preloaded parameters (weight, age)
- Start a workout by typing in my ID
- Start a workout by talking to alexa or google
- Start a scenario with the right music / ambience
- Lose more fat
- Gain more muscles

Greetings.

Tom De Vlaminck.

