using System;
using System.Collections.Generic;
using System.Text;

namespace System.Devices.Gpio
{
    /// <summary>
    /// GPIO - Basic Scenarios
    /// 
    /// Most GPIO implementations share a core set of functionality to allow basic on/off control
    /// of pins. At the very least, we should support these.
    /// - Open a object that is a representation of a pin with the given pin number
    /// - Support closing a pin to release the resources owned by that pin object
    /// - Represent the mode of the pin that details how the pin handles reads and writes (e.g. Input, Output)
    /// - Allow a resistor to be added to a pin such that it can be set as pullup or pulldown (or no pull)
    /// - Support setting a PWM value on a pin
    /// </summary>
    #region GPIO - Basic Scenarios

    public partial class GPIOPin
    {
        int PinNumber { get { throw new NotImplementedException(); } }
        bool Read() { throw new NotImplementedException(); }
        void Write(bool value) { throw new NotImplementedException(); }
        void Dispose() { throw new NotImplementedException(); }
        GPIOPin(GPIOController controller, int pinNumber, GPIOPinMode pinMode = GPIOPinMode.Input) { }

        GPIOPinMode PinMode { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        bool IsPinModeSupported(GPIOPinMode pinMode) { throw new NotImplementedException(); }
        int PWMValue { set { throw new NotImplementedException(); } }
    }

    public partial class GPIOController
    {
        GPIOPin OpenPin(int pinNumber) { throw new NotImplementedException(); }
        void ClosePin(int pinNumber) { throw new NotImplementedException(); }
        void ClosePin(GPIOPin pin) { throw new NotImplementedException(); }
        int PinCount() { throw new NotImplementedException(); }
        IEnumerable<GPIOPin> ConnectedPins { get { throw new NotImplementedException(); } }
    }

    enum GPIOPinMode
    {
        Input,
        Output,
        PWM,
        Pull_None,
        Pull_Down,
        Pull_Up,
    }

    #endregion

    /// <summary>
    /// GPIO - Advanced Scenarios
    /// - Analog Reads and Writes - Most GPIO works with digital pins, but sometimes analog pins are used. The difference in the 
    ///     Analog pins is that they have a range of potential values instead of just being on/off like the digital pins.
    /// - Listeners - Polling, interrupts, etc. There should be some way to listen for a change and respond accordingly
    /// - Edge Detection - Used with listeners/eventing as a way of definining the circumstances under which an event/callback will be raised
    /// - Allow setting a Debounce duration to ignore quickly occuring events during some timespan.
    /// </summary>
    #region GPIO - Advanced Scenarios

    public partial class GPIOPin
    {
        // Analog
        int AnalogRead() { throw new NotImplementedException(); }
        void AnalogWrite(int value) { throw new NotImplementedException(); }

        // Listeners
        // TODO

        // Edge Detection
        // TODO

        // Debounce
        public TimeSpan Debounce { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); } }
    }

    #endregion

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
        GPIOController(GPIOScheme numbering = GPIOScheme.BOARD) { throw new NotImplementedException(); }
    }

    public enum GPIOScheme
    {
        BOARD,
        BCM
    }

    public partial class GPIOPin
    {
        // Waiters
        public bool ReadWait(TimeSpan timeout) { throw new NotImplementedException(); }
        public int AnalogReadWait(TimeSpan timeout) { throw new NotImplementedException(); }

        // Bit-Shifts and writer helpers
        // TODO

        // Advanced PWM
        public int PWMRange { get { throw new NotImplementedException(); } }
        public PWMMode PWMMode { get { throw new NotImplementedException(); } }
    }

    public enum PWMMode
    {
        MARK_SPACE,
        BALANCED
    }

    #endregion

    #region  SPI, I2C, Serial

    public partial class GPIOPin
    {

    }

    #endregion
}
