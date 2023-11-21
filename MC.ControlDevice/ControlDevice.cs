using MC.BusinessObjects.Entities;
using MC.BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MC.ControlDevice
{
    public class ControlDeviceClass
    {
        #region  Variables

        public EventHandler DeviceMessageControlState;
        private SerialPort _ComPort = new SerialPort();
        StatesControl _StatesControl = new StatesControl();

        #endregion  Variables

        #region  Constructors

        public ControlDeviceClass()
        {
            _ComPort.DataReceived += new SerialDataReceivedEventHandler(_ComPort_DataReceived);
        }

        #endregion  Constructors

        #region comPort_DataReceived

        private void _ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            
            Thread.Sleep(500);
            string indata = _ComPort.ReadExisting();
            string[] RESULT = indata.Split('\n', '\r');
            string Response = RESULT[0].ToString();
            Actuar(Response);
            //EventArgsControlDevice u = new EventArgsControlDevice(_StatesControl, oResultadoOperacion);
            //DeviceMessageControlState(this, u);
        }
        private void Actuar(string Response)
        {
            ResultadoOperacion oResultadoOperacion = new ResultadoOperacion();

            try
            {

                if (Response.Length > 5)
                {

                    switch (_StatesControl)
                    {
                        case StatesControl.Conexion_Exitosa:
                            break;
                        case StatesControl.Error_Conexion:
                            break;
                        case StatesControl.BotonPresionado:
                            break;
                        case StatesControl.VehiculoMueble:

                            string Result = Response.Substring(0, 4);

                            if (Result == "#210" || Result == "#201" || Result == "#211")
                            {
                                if (Result == "#210" || Result == "#211")
                                {
                                    oResultadoOperacion.EntidadDatos = "Carro";
                                }
                                else if (Result == "#201")
                                {
                                    oResultadoOperacion.EntidadDatos = "Moto";
                                }

                                oResultadoOperacion.Mensaje = "Vehiculo en mueble";
                                _StatesControl = StatesControl.VehiculoMueble;
                                oResultadoOperacion.oEstado = TipoRespuesta.Exito;
                            }
                            break;
                        case StatesControl.NoHayCarro:
                            break;
                        case StatesControl.EstadoVehiculoTalanquera:
                            if (Response.Substring(5, 1) == "1")
                            {
                                _StatesControl = StatesControl.VehiculoTalanquera;
                                oResultadoOperacion.oEstado = TipoRespuesta.Exito;
                                oResultadoOperacion.Mensaje = "Vehiculo en talanquera";
                            }
                            break;
                        case StatesControl.EstadoVahiculoSalioTalanquera:
                            if (Response.Substring(5, 1) == "0")
                            {
                                _StatesControl = StatesControl.VahiculoSalioTalanquera;
                                oResultadoOperacion.oEstado = TipoRespuesta.Exito;
                                oResultadoOperacion.Mensaje = "Vehiculo salio de talanquera";
                            }
                            break;
                        case StatesControl.NoBoton:
                            break;
                        default:
                            break;
                    }


                    #region OLD

                    //if (Response == "#31101%")
                    //{
                    //    _StatesControl = StatesControl.BotonPresionado;
                    //    oResultadoOperacion.oEstado = TipoRespuesta.Exito;
                    //    oResultadoOperacion.Mensaje = "Botón Presionado";
                    //}
                    //else if (Response == "#31001%")
                    //{
                    //    //_StatesControl = StatesControl.NoBoton;
                    //    //oResultadoOperacion.oEstado = TipoRespuesta.Exito;
                    //    //oResultadoOperacion.Mensaje = "No Boton";
                    //}
                    //else
                    //{
                    //    string Result = Response.Substring(0, 4);
                    //    if (Result == "#200")
                    //    {
                    //        //_sMueble = false;
                    //        _StatesControl = StatesControl.NoHayCarro;
                    //        oResultadoOperacion.oEstado = TipoRespuesta.Exito;
                    //        oResultadoOperacion.Mensaje = "No hay carro";
                    //    }
                    //    else if (Result == "#210" || Result == "#201" || Result == "#211")
                    //    {
                    //        //_sMueble = true;
                    //        _StatesControl = StatesControl.VehiculoMueble;
                    //        oResultadoOperacion.oEstado = TipoRespuesta.Exito;

                    //        if (Result == "#210" || Result == "#211")
                    //        {
                    //            oResultadoOperacion.EntidadDatos = "Carro";
                    //        }
                    //        else if (Result == "#201")
                    //        {
                    //            oResultadoOperacion.EntidadDatos = "Moto";
                    //        }

                    //        oResultadoOperacion.Mensaje = "Vehiculo en mueble";

                    //    }
                    //    //string BB = Response.Substring(5, 1);
                    //    if (Response.Substring(5, 1) == "1")
                    //    {
                    //        //_sBarrera = true;
                    //        _StatesControl = StatesControl.VehiculoTalanquera;
                    //        oResultadoOperacion.oEstado = TipoRespuesta.Exito;
                    //        oResultadoOperacion.Mensaje = "Vehiculo en talanquera";
                    //    }
                    //    //else if (Response.Substring(5, 1) == "0")
                    //    //{
                    //    //    _StatesControl = StatesControl.VahiculoSalioTalanquera;
                    //    //    oResultadoOperacion.oEstado = TipoRespuesta.Exito;
                    //    //    oResultadoOperacion.Mensaje = "Vehiculo salio de talanquera";
                    //    //}
                    //}
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _StatesControl = StatesControl.Error_Conexion;
                oResultadoOperacion.oEstado = TipoRespuesta.Error;
                oResultadoOperacion.Mensaje = "Error de Conexion " + ex.ToString();
            }

            EventArgsControlDevice u = new EventArgsControlDevice(_StatesControl, oResultadoOperacion);
            DeviceMessageControlState(this, u);
        }
        #endregion comPort_DataReceived

        public ResultadoOperacion ConectarControl(string sPuerto)
        {
            ResultadoOperacion oResultadoOperacion = new ResultadoOperacion();

            try
            {
                if (_ComPort.IsOpen)
                {
                    _ComPort.Close();
                }


                _ComPort.PortName = sPuerto;
                _ComPort.BaudRate = 9600;
                _ComPort.Parity = System.IO.Ports.Parity.None;
                _ComPort.DataBits = 8;
                _ComPort.Open();

                _StatesControl = StatesControl.Conexion_Exitosa;
                oResultadoOperacion.oEstado = TipoRespuesta.Exito;
                oResultadoOperacion.Mensaje = "Conexion Exitosa";

            }
            catch (Exception ex)
            {
                _StatesControl = StatesControl.Error_Conexion;
                oResultadoOperacion.oEstado = TipoRespuesta.Error;
                oResultadoOperacion.Mensaje = ex.ToString();
            }


            EventArgsControlDevice e = new EventArgsControlDevice(_StatesControl, oResultadoOperacion);
            DeviceMessageControlState(this, e);

            return oResultadoOperacion;

        }

        public ResultadoOperacion EstadoControl()
        {
            ResultadoOperacion oResultadoOperacion = new ResultadoOperacion();

            try
            {
                _ComPort.Write("#2E01%");
            }
            catch (Exception ex)
            {

            }

            EventArgsControlDevice e = new EventArgsControlDevice(_StatesControl, oResultadoOperacion);
            DeviceMessageControlState(this, e);

            return oResultadoOperacion;
        }
        public ResultadoOperacion EstadoVehiculoMueble()
        {
            ResultadoOperacion oResultadoOperacion = new ResultadoOperacion();

            try
            {
                _ComPort.Write("#2E01%");
                _StatesControl = StatesControl.EstadoVehiculoMueble;
            }
            catch (Exception ex)
            {

            }

            EventArgsControlDevice e = new EventArgsControlDevice(_StatesControl, oResultadoOperacion);
            DeviceMessageControlState(this, e);

            return oResultadoOperacion;
        }
        public ResultadoOperacion EstadoVehiculoTalanquera()
        {
            ResultadoOperacion oResultadoOperacion = new ResultadoOperacion();

            try
            {
                _ComPort.Write("#2E01%");
                _StatesControl = StatesControl.EstadoVehiculoTalanquera;
            }
            catch (Exception ex)
            {

            }

            EventArgsControlDevice e = new EventArgsControlDevice(_StatesControl, oResultadoOperacion);
            DeviceMessageControlState(this, e);

            return oResultadoOperacion;
        }
        public ResultadoOperacion EstadoVehiculoSalioTalanquera()
        {
            ResultadoOperacion oResultadoOperacion = new ResultadoOperacion();

            try
            {
                _ComPort.Write("#2E01%");
                _StatesControl = StatesControl.EstadoVahiculoSalioTalanquera;
            }
            catch (Exception ex)
            {

            }

            EventArgsControlDevice e = new EventArgsControlDevice(_StatesControl, oResultadoOperacion);
            DeviceMessageControlState(this, e);

            return oResultadoOperacion;
        }
        public ResultadoOperacion AperturaBarrera()
        {
            ResultadoOperacion oResultadoOperacion = new ResultadoOperacion();

            try
            {
                _ComPort.Write("#11X01%");
            }
            catch (Exception ex)
            {

            }

            EventArgsControlDevice e = new EventArgsControlDevice(_StatesControl, oResultadoOperacion);
            DeviceMessageControlState(this, e);

            return oResultadoOperacion;
        }
    }

    public class EventArgsControlDevice : EventArgs
    {
        private StatesControl _result;

        public StatesControl result
        {
            get { return _result; }
            set { _result = value; }
        }

        private ResultadoOperacion _resultString;

        public ResultadoOperacion resultString
        {
            get { return _resultString; }
            set { _resultString = value; }
        }

        public EventArgsControlDevice(StatesControl oStatesControl, ResultadoOperacion oResultadoOperacion)
        {
            _result = oStatesControl;
            _resultString = oResultadoOperacion;
        }
    }
}
