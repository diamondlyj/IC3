using AIn.Ventures.BaseLibrary;
using AIn.Ventures.BaseLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.ain.ventures.App_Start
{
    /*
    public class PopulateDBMethod
    {
        Random rnd = new Random();
        string[] technoBabble = "Abutment Acceleration Activation Advice Advise Amplitude Analysis Angle Assembly Automation Axis AxleBalance Battery Bearing Blueprint Building Calculation Cantilever Cell Circumference Combustion Communication Component Component Compress Concept Constriction Construction Consultation Control Conversion Conveyance Conveyor belt Cooling Coupling Crank Current Curves Degree Depth Design Device Diagram Diameter Diesel Dimension Direction Distill Distribution Elastic Electrical Electronics Element Ellipse Energy Engine Excavation Expert Fabrication Flexible Flow Fluid Fluorescent Force Frame Friction Fuel Fulcrum Gear Gears Generate Generator Gimbals Grade Grading Hardware Heat Hoist Horizontal Hydraulic Illumination Information Injection Installation Instrument Intersection Joint Lever Lift Liquid Load Machine Management Manufacturing Mark Measurement Mechanize Modular Mold Motion Motor Negative Nuclear Object Operation Oscilloscope Physics Pivot Plumb Pneumatic Power Precision Process Production Project Propulsion Pulley Radiate Ream Refine Refrigeration Regulation Repair Retrofit Revolution Rotation Savvy Scheme Schooling Scientific Sequence Shape Slide Solar Stability Strength Structure Structure Studying Superstructure Suspension Technology Tools Transform Transmission Transmit Turbine Vacuum Valve Vertical Vibration Weight Weld Withstand Worker Boss Broach Bushing Cam Casting Chamfer Clevis  A  A to D  A-D  A-h  A-weighted dB levels  A-Weighting  A/D  A/D Converter  A/D Mux  Accelerometer  Access Point Base Station  ACPI  ACPR  ACR  ADC  Add-Drop Mux  ADM  ADMs  ADPCM  ADS  ADSL  Advanced Configuration  Advanced Mobile Phone   Advanced Power Management  Advanced Product Quali  AEC-Q100  AECQ100  AFE  AGC  Ah  Air Discharge  Air Gap Discharge  Air-Gap Discharge Method  AIS  AISG  alias  Aliasing  Alternator  AM  Ambient Sensor  Ambient Temperature  Ambient Temperature Se  American National Stan  American Wire Gauge  AMLCD  Amp  amp-hour  Ampacity  Ampere  Ampere-hour  ampere-second  Amplifier  Amplifier Class  Amplitude Modulation  AMPS  AMR  Analog  Analog Fan Controller  Analog Front End  Analog Switch  Analog Temperature Sensor  Analog to Digital  analogue  AND  ANSI  Antenna Interface Stan  anti-alias  Anti-Aliasing  APC  APD  API  APM  APON  APQP  Arrhenius  Arrhenius/FIT Rate  ASCII  ASIC  Asymmetric Digital Sub  ATE  ATM  Audio Taper  Auto Shutdown  Auto Shutdown +  Autoclave Test  Automatic Gain Control  Automatic Meter Reading  Automatic Power Control  AutoShutdown  Autoshutdown Plus  AutoShutdown+  Autotransformer  Avalanche Photo Diode  Avalanche Photodiode  AWG   B B  B-Field  Backup Step-Up  Balanced  band  Bandwidth  Base Station  Base Transceiver Station  Baseband  Baseline  Basestation  Basic Spacing between   Bass Boost  Battery Backed  Battery Backup  Battery Bridge  Battery Freshness Cap  Battery Freshness Seal  Battery Fuel Gauge  Battery Monitor  Battery Switchover  BBM  BCD  Bel  BER  BERT  Beyond-the-Rails™  BGA  bi-phase modulation  Bidirectional  Binary phase-shift keying  Bipolar Inputs  Bipolar Junction Trans  BIST  Bit Banging  Bit Error Rate  Bit Error Rate (BER) T  Bit Error Ratio  BJT  Blade Server  Blink Control  BLM  Bluetooth  BLVDS  BOC  Boost  Boost Controller  Boost Converter  Boost DC-DC  Boost DC/DC  Booster  Bootstrap  Bounded Jitter  BPON  bps  BPSK  BR  BRD  Break-Before-Make  BRI  Bridge Battery  Bridge Tied Load  Bridge-Battery  Bridge-Tied Load  Bridged Battery  Bridged Tied Load  Brightness  Broadband  Brown and Sharpe wire   Brownout  BSC  BSLF  BT  BTL  BTS  Buck  Buck Controller  Buck DC-DC  Buck Regulator  Buck Switcher  Buck-Boost  Buck-Boost Converter  Buck-Boost Regulator  Buck/Boost  Buck/Boost Converter  Buck/Boost Regulator  Burst Dimming  Burst Mode  Burst Mode Applications  Bus  BW  BWLS  BWSS   C C  C/N  C13 connector  C14 connector  CA  Cable  Cable Television  Cable TV  CAD  CAN  capacitance  Capacitive Crosstalk  Capacitive Voltage Reg  Capacitor  Capacitor Charge Pumps  CardBus  Carrier-free  CAS  CAT 3  cat 5  CAT3  CAT5  Category 3  Category 5  CATV  CBR  CC/CV Charger  CCCv  CCD  CCFL  CCFLs  CCFT  CCK  CCM  CDC  CDD  CDF-AEC-Q100  CDIP  CDMA  CDR  CE Control  CE Gating  Cell-Site  centimeter  centimetre  CH  Ch. to Ch. Skew (Ps Max)  Channel Associated Sig  Chans.  Charge Coupled Device  Charge Injection  Charge Pump  Charge Termination Method  CHATEAU  Chip  Chip-Enable Gating  Chipping Rate  chroma  Chrominance  CID  CIM  Circuit Board  CISC  Class  Class A  Class AB  Class B  Class C  Class D  Class G  Class H  click  Click-and-Pop  Click/Pop Reduction  Clock and Data Recovery  Clock Distribution Device  Clock Distribution Driver  Clock Jitter  Clock Recovery  Clock Recovery Data  Clock Reduction  Clock Throttling  cm  CMF  CMI  CML  CMOS  CMR  CMRR  CMVR  CNC  CO  Code Division Multiple  CODEC  COG  Coherent Sampling  COLC  Cold Cathode Fluoresce  Cold Cathode Fluoresce  color signal  Color Subcarrier  Column-Address-Strobe  Common Mode Rejection   Common-Mode  Common-Mode Signals  comp  Comp. Prop. Delay  compander  companding  Comparator  complementary  Complete Central Offic  Complex instruction se  CompoNet  Constant Current Const  Contact Bounce  Contact Discharge  Controller  Controller Area Network  Coplanar Line  Coulomb  Coulomb Counter  Coulomb Counting  CP  CPGA  cps  CPU Throttling  CR  CRC  CRIL  Cross Talk  Crossover  Crosstalk  Crowbar Circuit  CRT  Cryptanalysis  CS  CSA  CSP  CTIM  CTON  current  current controlled vol  Current Mode Feedback  Current Mode PWM  Current Sensing  Current Sensor  Current-Mode Controller  Current-Sense Amplifier  cycles  Cyclic Redundancy Check   D D-Pot  D/A  D/A Converter  DAC  Daisy Chain  Daisy-Chain  Dallastat  Damping factor  DAQ  DAS  Data Acquisition System  Data Converter  Data Management Language  Data Manipulation Lang  Data Over Cable Servic  Daylight Running  Daytime Running Lamp  Daytime Running Light  dB  dBA  dBm  DBS  DC  DC-DC  DC-DC Controller  DC-DC Converter  DC-DCs  DC/DC  DCE  DCM  DCR  DCS  DDI  DDJ  DDR  DDR Memory  DDR RAM  DDR-SDRAM  DDRAM  DDRD  DDS  Debounce  Debounced  Debouncing  Decibel  DECT  DeepCover  Delay Line  Delta-Sigma  Delta-Sigma Converter  Dense Wave Division Mu  Design for Testability  Deterministic Jitter  Development Kit  DFE  DFMEA  DFT  DG  Differential  Differential Non-Linea  Differential Nonlinearity  Differential Remote Ou  Differential Signaling  Digipot  digital amplifier  Digital Audio Signal P  Digital Cellular System  Digital Fan Control  Digital Log Pot  Digital Multimeter  Digital Pot  Digital Potentiometer  Digital Resistors  Digital Signal Processor  Digital Subscriber Line  Digital Video Broadcast  Digital-To-Analog Conv  DIO  Diode  DIP  Direct Broadcast Satel  direct digital synthesis  Direct Memory Access  Direct-Sequence Spread  Discrete Fourier Trans  Distortion  Dithering  DIU  Diversity  DJ  DLC  Dlog Pot  DMA  DML  DMM  DMMs  DMP  DMR  DMT  DNL  DOCSIS  Double Data Rate Synch  Doublers  Down Converter  Down Converters  Downconverter  DP  DPAK  DPD  DPDT  DPH  DPL  DPM  DPOT  DPS  DPST  DPWM  DQPSK  Drain  DRAM  DRC  DRL  Drypack  DSL  DSLAM  DSP  DSSP  DSSS  DTB  DTE  DTMF  Dual In-line Package  Dual Mode  Dual Phase Controller  Dual Tone Multiple Fre  Dual-Band  Dual-Modulus Prescaler  DVB  DVM  DWDM  DXC  Dynamic RAM  Dynamic Range   E E  E1  E2  E3  EAM  ECB  ECL  ECM  EconoReset  EconOscillator  EDFA  EDGE  EEPROM  Effective Number Of Bits  Effective Series Induc  Effective Series Resis  EFT  EIA  EIA-422  EIA-485  EIA-JEDEC  EIA232  Electric Vehicle Servi  Electro-Absorption Mod  Electromagnetic Interf  Electromotive Force  Electronic Digital Rhe  Electronic Industries   Electrostatic Discharge  Embedded System  EMC  EMF  EMI  End Point  ENDEC  Energy Harvesting  Energy Scavenging  Enhanced Data Rates Fo  ENOB  EP  EPON  EPROM  Equivalent Series Indu  Equivalent Series Resi  ERC  Error Vector Magnitude  ESBGA  ESD  ESD Protected  ESD Protection  ESF  ESL  ESP  ESR  Ethernet  Euroconnector  EV  EV Kit  Eval Kit  Evaluation Kit  Evaluation System  EVKIT  EVM  EVSE  EVSYS  Exposed Pad  Exposed Paddle  Extended Superframe   F F  fA  Facility Data Link  Fail-Safe  Fan Control  Fan Controller - Linear  Fan Controller - PWM  Farad  Fast Fourier Transform  Fault Blanking  Fault Tolerant  FB  FCD  FCR  FDD  FDDI  FDL  FDM  FE  FEC  femto  Femto Base Station  femto basestation  femtobasestation  femtocell  FET  FFT  FG  FHSS  Fiber Channel  Fiber Distributed Data  Fiber-to-the-node  Fibre Channel  Field Programmable Gat  Field-Effect Transistor  Fieldbus  FIFO  Finagle's Law  FireWire  First In First Out  FIT  Flash ADC  Flash ADCs  FlexSound  Floating  FM  FM Modulator  FOC  Foldback Current Limit  Foldback Mode  folded-frequency  Force-Sense  Forward Converter  Forward Error Correction  Fourier Transform  FOX  FPBW  FPGA  FR  Frame Relay  Framer  Framers  Framing  Frequency Bin  Frequency Diversity  Frequency Division Mul  Frequency Hopping  Frequency Hopping Spre  Frequency Modulation  Frequency Shift Keying  Frequency Synthesizer  FS  FSC  FSK  FSO  FSOTC  FSR  FTC  FTCL  FTTB  FTTH  FTTN  Fuel Gauge  Full Duplex  Full-Duplex   G G  GaAs  GaAs MESFET  GaAsFET  GaAsP  Gain  Gain Error  Gallium Arsenide  Gallium Arsenide Metal  Gallium Arsenide Phosp  Galvanic Isolation  gamma  gamma compensation  Gamma Correction  Gate  Gaussian frequency-shi  Gaussian minimum shift  GbE  GBIC  GBW  General Packet Radio S  General Purpose I-O  General Purpose I/O  General Purpose Interf  Generator  GFSK  GHz  Gigabit  Gigabit Interface Conv  Glitch  Glitch Immunity  Global Navigation Sate  Global Positioning System  Global System For Mobi  GLONASS  GMSK  GMSL  GPIB  GPIO  GPON  GPRS  GPS  GSM  GSM900  GT  GT/s  GUI   H H  H-Bridge  Half-Duplex  Half-Flash  Handover  Harmonic Distortion  HART  HAST  HB LED  HBT  HD  HD2  HDLC  HDLC Controller  HDSL  HDTV  Heat Sink  Heating-Ventilation-Ai  heatsink  HEMT  Hertz  HF  HGLL  Hi-Z  High Bit-Rate Digital   high impedance  High-Brightness LED  High-Definition Televi  High-Side  High-Speed Downlink Pa  High-Speed Packet Access  High-Speed Serial Inte  High-Speed Uplink Pack  High-Z  Home RF  Home-Rf  HomePlug  HomeRF  hot carrier diode  Hot-Swap  HotSwap  HP-IB  HR  HSDPA  HSPA  HSSI  HSUPA  HTML  HTS  HTTP  Human Body Model  HVAC  Hz   I I  I²C  I²S  I-Link  I.M.V.P  I/O  I/Q  IBO  IC  IC Foundry  ICA  ICR  ICVS  Ideal Factor  Ideality Factor  Idle Mode™  IEC  IEC connector  IEC-320  IEEE  IEEE 1394  IEEE 802.11g  IEEE 802.15.4  IEEE 802.16  IEEE P 1451.4  IEEE-1394  IEEE-488  IEEE802.11  IEEE802.11a  IEEE802.11b  IERC  IF  IFM  IFT  IHS  IIC  III-V  IIP3  IIS  IM  IMA  Image Frequency  Image Rejection  IMD  Impedance  Impulse  IMVP  In-Rush  In-Rush Current  Inductive Kickback  Inductor-Based  Inductor-Based Switcher  Industrial Scientific   InfiniBand  Infrared  Infrared Data Association  InGaAs  Ingress Protection  INL  Input Back-Off  Input CMVR (V)  Input Voltage Range  Inrush  Inrush Current  Int. Ref.  Integral Non-Linearity  Integral Nonlinearity  Integrated circuit  Integrated Heat Spreader  Integrated Interchip S  Integrated Temperature  Intel Mobile Voltage P  Intellectual Property  inter-IC bus  Inter-IC Sound  Inter-Modulation Disto  Inter-Symbol Interference  Intergrated circuit  Interleave  Intermediate Frequency  Intermod  Intermodulation  Intermodulation Distor  Internal Temperature  International Electrot  International Standard  International Telecomm  Internet Protocol  Internet Service Provider  Intersymbol Interference  Inverter  Inverters  Inverting Controller  Inverting DC-DC Converter  Inverting Switching Re  IO-Link  IP  IP3  IQ  IR  IrDA  IRE  IRO  IRS  IRSA  IRSD  IS  ISA  ISI  ISM  ISO  ISO/TS-16949  ISO/TS16949:2002  ISP  ITU  IVR   J J  JALT  JBOD  JEDEC  JFET  JITT  Jitter  Joule  JPEG  JUGFET  Junction Diode Sensor  Junction FET  Junction Temp Sensor  Junction Temperature S  Just A Bunch of Disks  JVM   K k  K-V-M  Kanal+  kb  kbps  kcmil  Keep-Out Area  Keep-Out Zone  Keyboard Video Mouse  kg  kHz  kilobits  km  ksps  kVA  kVM  kW  kWh   L L-Band  LAN  LANs  Large-scale integration  Laser Diode Driver  Laser Driver  LC circuit  LCC  LCD  LDO  LDO Regulator Low Dro  Leaded Chip Carrier  Leadless Chip Carrier  Leakage Inductance  LED  Level Translator  LFSR  LGHL  Li  Li+  Li-Ion  Light-Emitting Diode  LIN  Line Regulation  Linear  Linear Amplifier  Linear Fan Control  Linear Feedback Shift   Linear Mode  Linear Regulator  Linear Taper  Lion  Lithium  Lithium batteries  Lithium Ion  Lithium-Ion  Lithium-ion batteries  LL  Lm  Lm/W  LMDS  LNA  LO  Load Regulation  Local Interconnect Net  Local Multipoint Distr  Local Temperature  Local Temperature Sensor  Log Pot  Logarithmic Pot  Logarithmic Potentiometer  Logarithmic Taper  LOL  Long Haul  Long Term Evolution  Long-Haul  LOP  LOS  Low Batt. Det.  Low Drop Out  Low Dropout  Low Dropout Linear Reg  Low Frequency Gain Boost  Low Line O/P  Low Noise Amplifier  Low Voltage Differenti  Low Voltage Emitter Co  Low Voltage Positive E  Low Voltage Transistor  Low-Side  LRC circuit  LSB  LSI  LTE  Luminance  LVC  LVDS  LVECL  LVPECL  LVS  LVTTL   M M2M  mA  MAC  MAC Address  maca  Machine-to-machine  machine-to-mobile  mAh  Make-Before-Break  Manchester Data Encoding  Manchester Encoding  manganese dioxide  manganese lithium  MAP  Margining  Master Out Slave In  Max. DNL (LSB)  Max. Hold Step (MV)  Max. INL as percent FSR  MaxBass  MAXTON  MBB  MBC  Mbps  MC  MCM  Mcps  MDAC  MEC  Media Access Control A  Media Independent Inte  MegaBaud  megabits  Megachips per Second  Megacycles per Second  Megahertz  MEMS  MESFET  Metal Oxide Varistor  metal whiskers  Metal-Semiconductor Fi  MFSK  MHz  Micro Energy Cell  Microamp  microampere  Microelectromechanical  MicroLAN  MicroMonitor™  Microprocessor Supervisor  MII  Milliamp  milliamp-hour  Milliampere  Millivolt  MIMO  Min LOS Sens.  Min Stable Closed Loop  MISI  MISO  ML  mm  MMI  Monotonic  MOSFET  MOSI  MOV  MPU  MPW  MQFP  mrad  ms  MSA  MSB  Msps  MT  MT/s  MTIMD  MTPR  Multi-Chip Module  Multipath  multipath interference  Multiple Input-Multipl  Multiplex  Multiplexer  Multiplexing  Multiplexor  Murphy's Law  mutual conductance  MUX  mV  mW  MW  MxTNI   N n-channel  nA  Nanovolt  NC  NF  NIC  Nickel Metal Hydride  NiMH  NMI  nMOS  NO  Non Return To Zero  Non-Interruptible Powe  Non-Volatile  Non-Volatile Power Supply  Nonvolatile  Nonvolatile Memory  Norton amplifier  Noxious Fumes  NPR  NRD  NRE  NRZ  ns  NTC  nth  NTSC  NV  nV  NV Memory  NV-S  nW  Nyquist   O OC  OC-48  OEM  OFC  OFDM  ohm  OLED  OLT  One Wire  OneWire  ONT  ONU  Op amp  opamp  Open-collector  Open-drain  operational amplifier  operational transcondu  Optical Network Termin  Optical Network Unit  OR  OR-ing  Orthogonal Frequency D  OTA  Output to Input Ratio  Overvoltage Protection  OVP   P p-channel  P-P  pA  PA  PAE  PAL  PAM  Parallel  Parallel Interface  Parasite Power  Part 18  Partition Locking  Passive Optical Network  PBC  pC  PC Board  PC Card  PCB  PCI  PCI Express  PCI-E  PCIe  PCM  PCMCIA  PCS  PCT  PDA  PDC  PDH  PDI  PDIP  PDJ  PDM  PDO  Peak Inverse Voltage  peak reverse voltage  PECL  Peltier Junction  Periodic Operating Poi  Peripheral Component I  Peritel  Personal Communication  Personal Digital Cellular  pF  PFD  PFI  PFM  PFMEA  PFO  PG  PGA  PGAs  Phase alternate line  Phase Jitter  Phase-shift keying  Pin Electronics  PIV  PKI  PLA  Plastic Leaded Chip Ca  PLC  PLCC  Plesiochronous Digital  PLL  PLM  PMIC  PMM  Pmod  pMOS  PMR  PN Temperature Sensor  PoE  point of load  Point-of-Load  POK  POL  poly-carbonmonofluoride  PON  pop  POP Analysis  Pop Reduction  pop-noise  POR  Positive Temperature C  Pot  Potentiometer  Power Added Efficiency  Power Amplifier  Power Fail  Power Fail Comparator  Power Fail Detector  Power Harvesting  Power Management Integ  Power Supply Rejection  Power-Fail Comparator  Power-Over-Ethernet  PowerCap  Powerline  Powerline Communications  PPAP  PPOT  PRBS  PRC  PRCM  Pre-Bias Soft Start  Preemphasis  Pressure Cooker Test  Pressure Pot Test  Printed Circuit Board  Private Mobile Radio  PRM  Process Failure Mode a  PROCHOT  PROCHOT#  Production Part Approv  PROFIBUS  Programmable Controller  Programmable Gain Ampl  Programmable Logic Con  PROM  PRT  PRV  PS  PSD  PSK  PSR  PSRR  PSW  PTC  Public Key Infrastructure  Pulse Code Modulation  Pulse-Amplitude Modula  Pulse-Frequency Modula  Push-Pull  PV-S  PVR  PWD  PWM  PWM Fan Control  PWM Temperature Sensor   Q Q  Q Factor  Q-Injection  Q-Pump  Q100  QAM  QFN  QFP  QPSK  QRSS  QS-9000  Qs9000  QSOP  QSPI  Quadrature  Quadrature Amplitude M  Quadrature Modulation  Quadrature Phase Shift  Quadriphase Phase Shif  Quality factor  Quantization  Quaternary Phase Shift  QuERC  Quiescent   R R  R-2R  RAC  Radio Frequency Identi  Radio Frequency Interf  RAID  rail-switching  Rail-to-Rail Input  Rail-to-Rail Input or   RAM  Random Jitter  RAR  RC  rcvr  RE  Real Time Clock  Received Signal Streng  Received Signal Streng  Receiver  Recording Industry Ass  Recovery Time  Redundant Array Of Ind  REF  Regulator  Relay  Remote Diode  Remote Temp Sensor  Remote Temperature  Remote Temperature Sensor  Request To Send  Resistance  Resistance Temperature  Resonant Circuit  Response Time  Return To Zero  Reverse Breakdown Voltage  Reverse Recovery Time  RF  RF ID  RFDS  RFI  RFID  RFPF  RH  RI  RIAA  Ripple Rejection  RISC  RJ  RMS  RNPF  Robbed Bit Signaling  ROM  RRC  RS-232  RS-232C  RS-422  RS-422/RS-485  RS-485  RS232  RS422  RS485  RSA  RSR  RSSI  RTC  RTCs  RTD  RTS  Rx  RZ   S S  S Parameters  S-Parameters  S-UMTS  S-video  S/N  S/N Ratio  S/S  Sample Rate  Samples per Second  Sampling Rate  SAN  SAR  SAW  SAW Filter  SAW Oscillator  SB  SBD  SBGA  SBS  scan chain  Scan Design  SCART  Scattering Parameters  SCF  Schottky barrier diode  Schottky Diode  SCL  SCLK  SCR  SCSI  SCSI Bus  SCSI Interface  SCSI Terminator  SCSI2  SCSI3  SCT  SCTs  Scuzzy  SD  SDA  SDH  SDO  SDRAM II  SDTV  Second Harmonic Distor  Secure Digital  Secure Hash Algorithm  Secure Hash Standard  Semiconductor  Sense Resister  Sense Resistor  Sense-Resister  Sense-Resistor  SEPIC  SerDes  Serial  Serial Interface  Serial Peripheral Inte  SFDR  SFF  SFF-8472  SFP  SFR  SHA  Shannon sampling frequ  SHDN  Shift Register  Shock Sensor  Shoot-Through Current  Shut Down  Shut-Down  Shutdown  SI  SiGe  Sigma-Delta  Signal Detect  Signal-Invalid O/P  Signal-To-Noise And Di  Signal-to-Noise Ratio  Silicon Germanium  Silicon Timed Circuit  SIM  SINAD  Single Ended Primary I  Single-Wire Serial Int  SLBI  SLIC  Small Form Factor  Small Form Factor Plug  Small Office/Home Office  Smart Battery  Smart Battery Specific  Smart Phone  Smart Signal Conditioner  Smartphones  SMBus  SMBus I/F  SMD  SMPS  SMR  Sn whiskers  SNR  Snubber  SO  SoC  Sod's Law  Soft Start  Soft-Start  SOHO  SOIC  Solid State  SONET  SOT  Space Diversity  SPC  SPCR  SPDR  SPDT  Specialized Mobile Radio  SPFP  SPI  SPICE  Spread Spectrum  sps  SPST  Spurious-Free  Spurious-Free Dynamic   SQC  SR  SRAM  SRF  SS  SSC  SSOP  Standard Definition Te  Standing Wave Ratio  Star Ground  Star Point  Static RAM  statistical process co  statistical quality co  STB  STC  Step Down DC DC  Step Down Regulator  Step Down Switcher  Step-Down DC DC  Step-Up DC-DC  Storage Area Network  Strobe  Subscriber-Loop-Interf  Successive Approximati  Superheterodyne Receiver  Supervisor  Supervisory  Surface Acoustic Wave  Surface Acoustic Wave   surge suppressor  Swallow Counter  SWAP  Switch  Switch Debouncer  Switch Mode  Switch Mode Controller  Switch-Mode Power Supply  Switched Cap  Switched Capacitor Cir  Switched Capacitor Con  Switcher  switching amplifier  Switching Regulator  SWR  SWT  Synchronous Digital Hi  Synchronous Optical Ne  Synchronous Rectification  Synchronous Rectifier  System Management Bus  System on a Chip  System Timing and Control   T T/H  T/R  T/s  T1  T1 Framer  T3  Tach  Tachometer  TAD  tank circuit  Taper  TC  TCP/IP  TCXO  Td-SCDMA  TDD  TDD WLAN  TDD-WCDMA  TDM  TDMA  TDMoIP  TDMoP  TDR  TDSCDMA  TEC  TEDS  Television  Temp  Temp Sensor  Tempco  Temperature  Temperature Comparator  Temperature Compensate  Temperature Control  Temperature Management  Temperature Resistor  Temperature Sensor  Temperature Shutdown  Temperature Switch  Tesla  TFT  THB  THD  THD+N  Thermal Control  Thermal Control Circuit  Thermal Management  Thermal Monitor  Thermal Shutdown  Thermal Switch  THERMDA  THERMDC  Thermistor  Thermochron  Thermochron i-Button  Thermochron iButton  Thermocouple  thermoelectric cooler  Thermostat  THERMTRIP  THERMTRIP#  THERMTRIP_L  Thin-QFN  THINERGY MEC  Third Order Input Inte  Third Order Intercept   Three-State  Through-Hole  TIA  TIM  Time Diversity  Time Division Multiple  Time Division Multiple  Timing Distortion  Tin Whiskers  TINI  TLA  Total Harmonic Distortion  Total Harmonic Distort  Totem Pole  TouchTone  TQFN  TQFP  Transceiver  Transconductance  Transconductance Ampli  Transducer Electronic   Transfer  transfer rate  Transformer  Transient Intermodulat  Transient Voltage Supp  Transimpedance Amplifier  Transistor  Transistor Sensor  Transistor Temperature  Transmission Control P  Transmission Gate  Transmitter  Transresistance Amplifier  trr  TS 16949  TS-16949  TSOC  TSOP  TSSM  TSSOP  TTC  TTFC  TTIMD  TTL  Tube Motor  Tubular Motor  TUE  tuned circuit  TV  TVM  TVS  Tweak  Twisted-Pair  Tx   U uA  UART  UBM  UCSP  UHF Filter  UI  Ultra High Frequency F  Ultra-Wideband  ULTRA160  UMTS  Uninterruptible Power   UniqueWare  UniqueWare Serialized  Universal Mobile Telec  Universal Serial Bus  UP Reset  UP Supervisor  Upconverter  Upconverters  UPS  Upverter  URL  USB  UV  UVLO  UWB".Split(' ');
        string[] rawcomponentName1 = { "", "", "", "Motorized ", "Electrical ", "Small ", "Large ", "Modular ", "Basic ", "Conductive " };
        string[] rawcomponentName2 = { "Ferro", "Super", "Thermo", "Ultra", "Sub", "Gyro", "Potenio", "Aero", "Chloro" };
        string[] rawcomponentName3 = { "meter", "conductor", "resistor", "magnet", "motor", "board", "foam", "gel" };
        string[] moduleName1 = { "", "", "", "Light-weight ", "Aerodynamic ", "Ergonomic ", "Articulated " };
        string[] moduleName2 = { "Arm ", "Leg ", "Hand ", "Body ", "Base ", "Auto-", "Circuit " };
        string[] moduleName3 = { "Cover", "Chassis", "Controller", "Board", "Framework", "System", "Armor" };
        string[] projectName1= { "Arm ", "Leg ", "Hand ", "Body ", "Base ", "Auto-", "Circuit " };
        public PopulateDBModel pop(int numberOfRecords)
        {
            List<AIn.Ventures.BaseLibrary.Component> Components = new List<AIn.Ventures.BaseLibrary.Component>();
            List<AIn.Ventures.BaseLibrary.ComponentToComponent> ComponentToComponents = new List<AIn.Ventures.BaseLibrary.ComponentToComponent>();
            List<Guid> UsedGuids = new List<Guid>();
            List<String> usedMfrs = new List<String>();
            List<AIn.Ventures.BaseLibrary.Project> Projects = new List<AIn.Ventures.BaseLibrary.Project>();
            Component cmp;
            Project project;
            ComponentToComponent ctc;
            Guid? parentGuid;
            Guid objectGuid;
            Guid projectGuid;
            Guid guid;
            string componentType;
            string name;
            string description;
            int amount;
            string manufacturer;
            string sku;
            string supplier;
            decimal price;
            string projectName;
            string projectDescription;
            double shares;


            

            for (int i = 0; i < numberOfRecords; i++)
            {
                if (UsedGuids.Count > 0 && rnd.Next(3) < 2)
                {
                    parentGuid = UsedGuids[rnd.Next(UsedGuids.Count)];
                    componentType = rnd.Next(4) < 3 ? "RawComponent" : "Module";
                }
                else
                {
                    parentGuid = null;
                    componentType = "Product";
                }

                objectGuid = Guid.NewGuid();
                if (componentType != "RawComponent")
                    UsedGuids.Add(objectGuid);

                amount = componentType.Equals("Product") ? 1 : rnd.Next(15);

                if (componentType == "RawComponent") name = generateRawcomponentName();
                else if (componentType == "Module") name = generateModuleName();
                else generateName(3);

                description = "THIS IS PLACEHOLDER DATA. Lorem ispum";//generateName(10);

                if (usedMfrs.Count > 0 && rnd.Next(4) < 3)
                {
                    manufacturer = usedMfrs[rnd.Next(usedMfrs.Count)];
                }
                else
                {
                    manufacturer = generateName(rnd.Next(3) + 1);
                }

                sku = generateSku();

                supplier = rnd.Next(2) < 1 ? "Amazon" : "Ebay";

                price = (decimal)rnd.Next(10000) / 100;
                cmp = new Component
                {
                    ObjectGUID = objectGuid,
                    Manufacturer = manufacturer,
                    ComponentType = componentType,
                    Description = description,
                  //  Name = name,
                    Price = price,
                    SKU = sku,
                    Supplier = supplier
                };
                if (parentGuid != null)
                {
                    ctc = new ComponentToComponent
                    {
                        ParentGUID = (Guid)parentGuid,
                        ObjectGUID = objectGuid,
                        amount = amount
                    };
                    ComponentToComponents.Add(ctc);
                }
                Components.Add(cmp);
            }
    
          


            return new PopulateDBModel { ComponentList = Components, CtcList = ComponentToComponents ,ProjectList=Projects};
        }

        private string generateName(int numWords)
        {
            string name = "";
            for (int i = 0; i < numWords; i++)
            {
                name += technoBabble[rnd.Next(technoBabble.Length)];
                if (i < numWords - 1)
                    name += ' ';
            }
            return name;
        }

        private string generateRawcomponentName()
        {
            return rawcomponentName1[rnd.Next(rawcomponentName1.GetLength(0))]
                + rawcomponentName2[rnd.Next(rawcomponentName2.GetLength(0))]
                + rawcomponentName3[rnd.Next(rawcomponentName3.GetLength(0))];
        }
        private string generateModuleName()
        {
            return moduleName1[rnd.Next(moduleName1.GetLength(0))]
                + moduleName2[rnd.Next(moduleName2.GetLength(0))]
                + moduleName3[rnd.Next(moduleName3.GetLength(0))];
        }
        private string generateProjectName()
        {
            return projectName1[rnd.Next(moduleName1.GetLength(1))];
        }
        private string generateString(int length)
        {
            string rtrnStr = "";
            for (int i = 0; i < length; i++)
            {
                rtrnStr += (char)(rnd.Next(94) + (int)' ');
            }
            return rtrnStr;
        }

        private string generateSku()
        {
            string sku = "";
            int skuLength = rnd.Next(10) + 7;
            int chunkLength = rnd.Next(3) + 2;
            for (int i = 0; i < skuLength; i++)
            {
                if (i < chunkLength)
                {
                    int numCapLc = rnd.Next(3);
                    switch (numCapLc)
                    {
                        case 0:
                            sku += (char)(rnd.Next(10) + (int)'0');
                            break;
                        case 1:
                            sku += (char)(rnd.Next(26) + (int)'a');
                            break;
                        default:
                            sku += (char)(rnd.Next(26) + (int)'A');
                            break;
                    }
                }
                else
                {
                    sku += '-';
                    chunkLength += rnd.Next(3) + 3;
                }
            }
            return sku;
        }
    }
    */
}


