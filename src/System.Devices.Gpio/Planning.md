# Goals
1. Research GPIO and gain a firm understanding of the practices and common use cases
2. Plan out an API for a CoreFX implementation of basic GPIO methods and take it through API Review
3. Complete a workable implementation of GPIO functionality
4. Functional (positive, negative) tests for 100% of Public APIs
5. BenchmarkDotNet performance tests written for 100% of Public APIs
6. Complete documentation for 100% of Public APIs, including usage examples

# Research GPIO
- Understand the basic protocols and vocabulary
    - Analog vs Digital pin numbering
    - BCM vs Board numbering on a raspberry pi
    - Pulse-Width Modulation (PWM)
    - Pullup/Pulldown resistors
	- Multi-pin communication protocols
		- SPI
		- I2C
		- UART (SerialPort)
- Look at other notable GPIO implementations documentation
	- [Windows.Devices.Gpio namespace (Windows IoT OS -only)](https://docs.microsoft.com/en-us/uwp/api/windows.devices.gpio)
	- [PI.IO (Corefxdev team hackathon project](https://github.com/Petermarcu/Pi)
	- [Wiring Pi (GPL)](http://wiringpi.com/)
	- [RPI.GPIO (MIT)](http://tieske.github.io/rpi-gpio/modules/GPIO.html)

- Check out some of these helpful articles:
    - [A Web developers guide to communication Protcols](https://tessel.io/blog/108840925797/a-web-developers-guide-to-communication-protocols)
    - [.NET Core on RPi](https://github.com/dotnet/core/blob/master/samples/RaspberryPiInstructions.md)
    - [PI.IO sample for reading barometric pressure, temperature, and more](https://github.com/Petermarcu/Pi/blob/master/IotSample/Program.cs)
    - [Sample program to use Alexa to control a Pi through Azure](https://github.com/Petermarcu/AlexaDotnetPi)
- Consider specific scenarios in which GPIO would be used in .NET Core. Some exmaples:
    - IoT Device Sends Data to Azure
		- Example: A Raspberry Pi3 device collects sensor data (room temperature, soil temperature, soil moisture) and uploads it to Azure
			- Azure IoT collects the data and turns the log data into graphs for an Azure hosted .NET Core Website (ASP.NET Core Razer page) and a Xamarin iOS/Android App
	- IoT Device Receives Data from Azure
		- Example: Azure IoT Hub / Azure Web Site (and phone app) can set alert triggers and events based on the data
			- When temperature is high/low, it sends the device a control signal to turn on/off a fan
            - When the moisture level is low/high, it sends the device a control signal to turn on/off a water supply

# API Design Requirements
## Basic Scenarios
Most GPIO implementations share a core set of functionality to allow basic on/off control
of pins. At the very least, we should support these.
- Open a object that is a representation of a pin with the given pin number
- Support closing a pin to release the resources owned by that pin object
- Represent the mode of the pin that details how the pin handles reads and writes (e.g. Input, Output)
- Allow a resistor to be added to a pin such that it can be set as pullup or pulldown (or no pull)
- Support setting a PWM value on a pin

Here's a rough API shape that fulfills those requirements:
```
namespace System.Devices.Gpio
{
    public partial class GPIOPin : IDisposable
    {
        int PinNumber { get { } }
        bool Read() { }
        void Write(bool value) { }
        void Dispose() { }
        GPIOPin(GPIOController controller, int pinNumber, GPIOPinMode pinMode = GPIOPinMode.Input) { }

        GPIOPinMode PinMode { get { } set { } }
        bool IsPinModeSupported(GPIOPinMode pinMode) { }
        int PWMValue { set { } }
    }

    public partial class GPIOController : IDisposable
    {
        GPIOPin this[int pinNumber] { get { } }
        GPIOPin OpenPin(int pinNumber) { }
        void ClosePin(int pinNumber) { }
        void ClosePin(GPIOPin pin) { }
        int PinCount() { }
        IEnumerable<GPIOPin> ConnectedPins { get { } }
        void Dispose() { }
    }

    public enum GPIOPinMode
    {
        Pull_Down,
        Pull_Up,
        PWM,
        Input,
        Output
    }
}
```
## Advanced Scenarios
Beyond the basic set of functionality are a set of functions that are supported by *almost* every implementation out there. They are:
- Analog Reads and Writes - Most GPIO works with digital pins, but sometimes analog pins are used. The difference in the 
    Analog pins is that they have a range of potential values instead of just being on/off like the digital pins.
- Listeners - Polling, interrupts, etc. There should be some way to listen for a change and respond accordingly
- Edge Detection - Used with listeners/eventing as a way of definining the circumstances under which an event/callback will be raised
- Allow setting a Debounce duration to ignore quickly occuring events during some timespan.

Stub API that builds on top of the previous API:
```
namespace System.Devices.Gpio
{
    public partial class GPIOPin
    {
        // Analog
        int AnalogRead() { }
        void AnalogWrite(int value) { }

        // Listeners
        // TODO

        // Edge Detection
        // TODO

        // Debounce
        public TimeSpan Debounce { get { }  set { } }
    }

}
```

```
namespace System.Devices.Gpio
{
    /// <summary>
    /// GPIO - Bonuses
    /// - Choose between BCM or BOARD pin numbering
    /// - Waiters - Instead of manually polling a Read, a Waiter will handle the polling until the desired Read value is reached
    /// - Bit shifting - Add helpers to allow easily working with more usable data types
    /// - Advanced PWM functions can be added to allow setting range, rpi mode, etc.
    /// </summary>
    #region GPIO - Other Scenarios

    // BCM vs BOARD
    public partial class GPIOController
    {
        GPIOController(GPIOScheme numbering = GPIOScheme.BOARD) { }
    }

    public enum GPIOScheme
    {
        BOARD,
        BCM
    }

    public partial class GPIOPin
    {
        // Waiters
        public bool ReadWait(TimeSpan timeout) { }
        public int AnalogReadWait(TimeSpan timeout) { }

        // Bit-Shifts and writer helpers
        // TODO

        // Advanced PWM
        public int PWMRange { get { } }
        public PWMMode PWMMode { get { } }
    }

    public enum PWMMode
    {
        MARK_SPACE,
        BALANCED
    }
}
```

```
namespace System.Devices.Gpio
{
    /// <summary>
    /// Stretch - Advanced Multi-Pin Connections
    /// 
    /// This section holds connection types where more than one pin is used to transmit data. There
    /// are a ton of these, but the most commonly supported are SPI and I2C, followed by UART/SerialPort
    /// </summary>
    #region Stretch - Advanced Multi-Pin Connections

    public enum GPIOPinMode
    {
        Pull_Down,
        Pull_Up,
        PWM,
        Input,
        Output,
        SPI,
        I2C,
        UART, // serialport
        Unknown
    }

    public partial class SpiConnection
    {
        // https://github.com/Petermarcu/Pi/tree/master/Pi.IO.SerialPeripheralInterface
    }

    public partial class I2cConnection
    {
        // https://github.com/Petermarcu/Pi/tree/master/Pi.IO.InterIntegratedCircuit
    }

    public class UARTConnection
    {
        // A Linux or platform-agnostic serial port library would likely have to be distinct from our 
        // existing bloated Windows implementation. That wouldn't necessarily be a bad thing, though, as
        // we could add basic functionality like read/write/open/close without the weight of the Windows
        // implementation weighing it down.
    }
}
```
```
namespace System.Devices.Gpio
{
    /// <summary>
    /// Out of Scope - More Advanced Connections
    /// 
    /// Though there are a bunch of useful connections types, we can't implement them all at once.
    /// This section lists some more cool types that we should keep in the back of our mind though
    /// and pursue after the above are complete.
    /// </summary>
    #region Out of Scope - More Advanced Connections

    public class USBConnection
    {
        // We could include discovery of USB and even allow hot-swapping potentially. In addition to
        // allowing easy communication over the port, of course.
    }

    public class BluetoothConnection
    {
        // Communicating with devices over bluetooth has been a highly requested feature for a while now. 
    }
}
```



# Week-by-Week Completion Plan

# Documentation and Examples
- Examples should be in a form that is easily recognizable for someone familiar with rpi developement, regardless of their current programming environment.

# Contacts
1. Edgardo Zoppi (summer intern)
2. Ian Hays
3. Paulo Janotti 
4. Tomas Weinfurt 
5. Levi Broderick (intern coach)
6. Josh Free (intern manager)