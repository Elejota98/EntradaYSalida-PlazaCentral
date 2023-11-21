using EGlobalT.Device.SmartCard;
using EGlobalT.Device.SmartCardReaders;
using EGlobalT.Device.SmartCardReaders.Entities;
using MC.BusinessObjects.DataTransferObject;
using MC.BusinessObjects.Entities;
using MC.BusinessObjects.Enums;
using MC.ModuloEntrada.WinForm.FrontEnd.Tickets;
using MC.ModuloEntrada.WinForm.Presenter;
using MC.ModuloEntrada.WinForm.View;
using MC.Utilidades;
using Microsoft.Reporting.WebForms;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MC.ModuloEntrada.WinForm.FrontEnd
{
    public partial class frmPrincipal : Form, IView_Principal
    {

        #region Rutas

        //Audio
        private string _sPathBienvenido = string.Empty;
        private string _sPathAutoVencida = string.Empty;
        private string _sPathSinTarjetasDispensador = string.Empty;
        private string _sPathSistemaSuspendido = string.Empty;
        private string _sPathTarjetaInvalida = string.Empty;
        private string _sPathTarjetaSinRegistroSalida = string.Empty;
        private string _sPathVehiculoEntrando = string.Empty;
        private string _sPathMensualidadUso = string.Empty;

        #endregion

        #region Definiciones
        private bool _Moto = false;
        public bool Moto
        {
            get { return _Moto; }
            set { _Moto = value; }
        }
        private bool _ControlReady = false;
        public bool ControlReady
        {
            get { return _ControlReady; }
            set { _ControlReady = value; }
        }
        private bool _BotonPresionado = false;
        public bool BotonPresionado
        {
            get { return _BotonPresionado; }
            set { _BotonPresionado = value; }
        }
        private bool _VehiculoMueble = false;
        public bool VehiculoMueble
        {
            get { return _VehiculoMueble; }
            set { _VehiculoMueble = value; }
        }
        private bool _VehiculoTalanquera = false;
        public bool VehiculoTalanquera
        {
            get { return _VehiculoTalanquera; }
            set { _VehiculoTalanquera = value; }
        }
        private bool _VehiculoSalioTalanquera = false;
        public bool VehiculoSalioTalanquera
        {
            get { return _VehiculoSalioTalanquera; }
            set { _VehiculoSalioTalanquera = value; }
        }
        private Int32 m_lUserID = -1;
        private Int32[] m_lAlarmHandle = new Int32[200];
        private Int32 iListenHandle = -1;
        private int iDeviceNumber = 0; //添加设备个数
        private uint iLastErr = 0;
        private string strErr;
        private CHCNetSDK.MSGCallBack_V31 m_falarmData_V31 = null;
        private CHCNetSDK.MSGCallBack m_falarmData = null;
        public delegate void UpdateListBoxCallback(string strAlarmTime, string strDevIP, string strAlarmMsg);

        bool bUso = false;
        Lectora_CRT602U Lector = new Lectora_CRT602U();
        Rspsta_Conexion_LECTOR RspConexion = new Rspsta_Conexion_LECTOR();
        Rspsta_DetectarTarjeta_LECTOR RspDetectar = new Rspsta_DetectarTarjeta_LECTOR();
        Rspsta_CodigoTarjeta_LECTOR RspIdCard = new Rspsta_CodigoTarjeta_LECTOR();
        Rspsta_Leer_Tarjeta_LECTOR RspLeerCard = new Rspsta_Leer_Tarjeta_LECTOR();
        private frmPrincipal_Presenter _frmPrincipal_Presenter;
        Transaccion oTransaccion = new Transaccion();
        string IdTarjeta = string.Empty;
        private int _cnt_timeout;
        public int cnt_timeout
        {
            get { return _cnt_timeout; }
            set { _cnt_timeout = value; }
        }
        private Pantalla _Presentacion = Pantalla.SalvaPantallas;
        private bool _KytReady = false;
        public bool KytReady
        {
            get { return _KytReady; }
            set { _KytReady = value; }
        }
        private bool _CardKytReady = false;
        public bool CardKytReady
        {
            get { return _CardKytReady; }
            set { _CardKytReady = value; }
        }
        private bool _CleanCardKyt = false;
        public bool CleanCardKyt
        {
            get { return _CleanCardKyt; }
            set { _CleanCardKyt = value; }
        }
        private bool _RemoveCard = false;
        public bool RemoveCard
        {
            get { return _RemoveCard; }
            set { _RemoveCard = value; }
        }
        private string _IdCard = string.Empty;
        public string IdCard
        {
            get { return _IdCard; }
            set { _IdCard = value; }
        }
        private Tarjeta _Tarjeta = new Tarjeta();
        public Tarjeta Tarjeta
        {
            get { return _Tarjeta; }
            set { _Tarjeta = value; }
        }
        private bool _WriteCard = false;
        public bool WriteCard
        {
            get { return _WriteCard; }
            set { _WriteCard = value; }
        }
        private bool _CRTReady = false;
        public bool CRTReady
        {
            get { return _CRTReady; }
            set { _CRTReady = value; }
        }
        private string _IdCardAutorizado = string.Empty;
        public string IdCardAutorizado
        {
            get { return _IdCardAutorizado; }
            set { _IdCardAutorizado = value; }
        }
        private string _IdTransaccion = string.Empty;
        public string IdTransaccion
        {
            get { return _IdTransaccion; }
            set { _IdTransaccion = value; }
        }
        string SecuenciaTransaccion = string.Empty;
        private List<DtoAutorizado> _lstDtoAutorizado = new List<DtoAutorizado>();
        public List<DtoAutorizado> lstDtoAutorizado
        {
            get { return _lstDtoAutorizado; }
            set { _lstDtoAutorizado = value; }
        }
        private bool _CicloActivo = false;
        public bool CicloActivo
        {
            get { return _CicloActivo; }
            set { _CicloActivo = value; }
        }
        private DtoModulo _DtoModulo = new DtoModulo();
        public DtoModulo DtoModulo
        {
            get { return _DtoModulo; }
            set { _DtoModulo = value; }
        }
        private Transaccion _Transaccion = new Transaccion();
        int CntAuto = 0;
        bool soundSuspen = false;
        int Ssuspe = 0;
        bool soundSinTar = false;
        int SsinTar = 0;
        bool InfoModulo = false;
        private bool _SinTarjetas = false;
        public bool SinTarjetas
        {
            get { return _SinTarjetas; }
            set { _SinTarjetas = value; }
        }
        bool Camara = false;
        bool VehiculoMoto = false;
        private List<DtoTarjetas> _lstDtoTarjetas = new List<DtoTarjetas>();
        public List<DtoTarjetas> lstDtoTarjetas
        {
            get { return _lstDtoTarjetas; }
            set { _lstDtoTarjetas = value; }
        }
        public DateTime _FechaCompleta = new DateTime();
        private string _Barrera = string.Empty;
        public string Barrera
        {
            get { return _Barrera; }
            set { _Barrera = value; }
        }
        private string _sPlaca = string.Empty;
        public string sPlaca
        {
            get { return sPlaca; }
            set { sPlaca = value; }
        }
        private string _PlacaRegistrada = string.Empty;
        public string PlacaRegistrada
        {
            get { return _PlacaRegistrada; }
            set { _PlacaRegistrada = value; }
        }

        private string _imgUrl = string.Empty;
        public string imgUrl
        {
            get { return _imgUrl; }
            set { _imgUrl = value; }
        }

        #endregion

        #region EventosFormulario
        public frmPrincipal()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            _frmPrincipal_Presenter = new frmPrincipal_Presenter(this);
        }
        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            TbTag.GotFocus += TabGotFocus;

            Inicio();
        }
        #endregion

        #region EventosControles
        private void tmrTimeOut_Tick(object sender, EventArgs e)
        {          
            SsinTar++;
            Ssuspe++;
            cnt_timeout++;

            switch (_Presentacion)
            {
                  
                case Pantalla.SalvaPantallas:
                    TbTag.Focus();
                    //PLACAS

                                General_Events = "(FrontEnd Antes CapturaPlaca)";
                                CapturaPlacaNew();
                                if (_frmPrincipal_Presenter.ValidarPlacaEntrada())
                                {
                                    if (_PlacaRegistrada == _sPlaca)
                                    {
                                        CapturaPlacaNew();
                                    }
                                    if (_sPlaca == _PlacaRegistrada)
                                    {
                                        _sPlaca = "------";
                                    }
                                }
                                General_Events = "(FrontEnd Antes CapturaPlaca) " + _sPlaca;

                    _SinTarjetas = false;
  
                        if (Convert.ToBoolean(Globales.sPLC) == true)
                        {

                            if (_frmPrincipal_Presenter.VehiculoMueble())
                            {


                                General_Events = "(FrontEnd Antes AlistarTarjetaNew)";
                                _frmPrincipal_Presenter.AlistarTarjetaNew();
                                General_Events = "(FrontEnd Despues AlistarTarjetaNew)";       

                                SoundPlayer simpleSound = new SoundPlayer(_sPathBienvenido);
                                simpleSound.Play();

                                SecuenciaTransaccion = DateTime.Now.ToString("yyyyMMddHHmmss") + Globales.sCarril + Globales.iCodigoEstacionamiento;

                                string ano = SecuenciaTransaccion.Substring(0, 4);
                                string mes = SecuenciaTransaccion.Substring(4, 2);
                                string dia = SecuenciaTransaccion.Substring(6, 2);
                                string hora = SecuenciaTransaccion.Substring(8, 2);
                                string min = SecuenciaTransaccion.Substring(10, 2);
                                string seg = SecuenciaTransaccion.Substring(12, 2);

                                string FechaCompleta = dia + "/" + mes + "/" + ano + " " + hora + ":" + min + ":" + seg;

                                _FechaCompleta = Convert.ToDateTime(FechaCompleta);

                            CapturaPlacaNew();
                            {
                                if (_PlacaRegistrada == _sPlaca)
                                {
                                    CapturaPlacaNew();
                                }
                                if (_sPlaca == _PlacaRegistrada)
                                {
                                    _sPlaca = "------";
                                }
                            }
                            General_Events = "(FrontEnd CapturaPlaca) " + _sPlaca;

                                if (_sPlaca != string.Empty && _sPlaca != "------")
                                {
                                    bool bAutorizado = false;
                                    bool ok2 = false;
                                    bool bAutoVencida2 = false;

                                    #region validacionAutorizado

                                    Autorizado oAutorizado = new Autorizado();
                                    oAutorizado.PlacaAuto = _sPlaca;
                                    General_Events = "oAutorizado.PlacaAuto : " + oAutorizado.PlacaAuto;

                                    if (_frmPrincipal_Presenter.ObtenerAutorizado(oAutorizado))
                                    {
                                        ResultadoOperacion oResultadoOperacion = new ResultadoOperacion();

                                        for (int i = 0; i < _lstDtoAutorizado.Count; i++)
                                        {
                                            if (_lstDtoAutorizado[i].IdEstacionamiento == Convert.ToInt64(Globales.iCodigoEstacionamiento))
                                            {

                                                if (_lstDtoAutorizado[i].EstadoAutorizacion && _lstDtoAutorizado[i].Estado && DateTime.Now >= _lstDtoAutorizado[i].FechaInicial && DateTime.Now <= _lstDtoAutorizado[i].FechaFinal)
                                                {
                                                    if (_frmPrincipal_Presenter.ValidarIngresoAuto(_lstDtoAutorizado[i].IdTarjeta))
                                                    {
                                                        ok2 = true;
                                                        bAutoVencida2 = false;
                                                        CntAuto = i;
                                                        break;
                                                    }
                                                    else
                                                    {

                                                        #region OLD
                                                        //simpleSound = new SoundPlayer(_sPathTarjetaSinRegistroSalida);
                                                        //simpleSound.Play();
                                                        //Presentacion = Pantalla.TarjetaSinRegistroSalida;
                                                        //break;
                                                        ////General_Events = "TarjetaSinRegistroSalida";
                                                        //////ok = true;
                                                        //////bAutoVencida = false;
                                                        //////bTarjetaInvalida = false;
                                                        //////CntAuto = i;
                                                        //////break;
                                                        #endregion
                                                        simpleSound = new SoundPlayer(_sPathMensualidadUso);
                                                        simpleSound.Play();
                                                        _IdCardAutorizado = "USO";

                                                        if (_Moto)
                                                        {
                                                            VehiculoMoto = true;

                                                            //escribir
                                                            _Tarjeta.ActiveCycle = true;
                                                            _Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;
                                                            _Tarjeta.EntrancePlate = _sPlaca;
                                                            _Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                            _Tarjeta.EntranceModule = Globales.sSerial;

                                                            _frmPrincipal_Presenter.Escribirtarjeta();
                                                        }
                                                        else
                                                        {
                                                            VehiculoMoto = false;

                                                            //escribir
                                                            _Tarjeta.ActiveCycle = true;
                                                            _Tarjeta.EntrancePlate = _sPlaca;
                                                            _Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;
                                                            _Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                            _Tarjeta.EntranceModule = Globales.sSerial;

                                                            _frmPrincipal_Presenter.Escribirtarjeta();
                                                        }
                                                        //Presentacion = Pantalla.TarjetaSinRegistroSalida;
                                                        break;
                                                        ////General_Events = "TarjetaSinRegistroSalida";
                                                        ////ok = true;
                                                        ////bAutoVencida = false;
                                                        ////bTarjetaInvalida = false;
                                                        ////CntAuto = i;
                                                        ////break;
                                                    }
                                                }
                                                else
                                                {
                                                    simpleSound = new SoundPlayer(_sPathAutoVencida);
                                                    simpleSound.Play();
                                                    Presentacion = Pantalla.AutorizacionVencida;
                                                    if (_frmPrincipal_Presenter.ValidacionMotoMueble() == false)
                                                    {
                                                        VehiculoMoto = true;

                                                        //escribir
                                                        _Tarjeta.ActiveCycle = true;
                                                        _Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                        _Tarjeta.EntranceModule = Globales.sSerial;
                                                        _Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;

                                                        _frmPrincipal_Presenter.Escribirtarjeta();
                                                    }
                                                    else
                                                    {
                                                        VehiculoMoto = false;

                                                        //escribir
                                                        _Tarjeta.ActiveCycle = true;
                                                        _Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                        _Tarjeta.EntranceModule = Globales.sSerial;
                                                        _Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;

                                                        _frmPrincipal_Presenter.Escribirtarjeta();
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                //Presentacion = Pantalla.TarjetaInvalida;
                                                //bTarjetaInvalida = true;
                                            }
                                        }

                                        if (ok2)
                                        {
                                            RegistroEntradaAutorizado(CntAuto);
                                        }
                                        else if (bAutoVencida2)
                                        {
                                            simpleSound = new SoundPlayer(_sPathAutoVencida);
                                            simpleSound.Play();
                                            Presentacion = Pantalla.AutorizacionVencida;
                                        }
                                    }
                                    else
                                    {
                                        //RegistroEntrada();
                                        if (_frmPrincipal_Presenter.ValidacionMotoMueble() == false)
                                        {
                                            VehiculoMoto = true;

                                            //escribir
                                            _Tarjeta.ActiveCycle = true;
                                            _Tarjeta.DateTimeEntrance = _FechaCompleta;
                                            _Tarjeta.EntranceModule = Globales.sSerial;
                                            _Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;

                                            _frmPrincipal_Presenter.Escribirtarjeta();
                                        }
                                        else
                                        {
                                            VehiculoMoto = false;

                                            //escribir
                                            _Tarjeta.ActiveCycle = true;
                                            _Tarjeta.DateTimeEntrance = _FechaCompleta;
                                            _Tarjeta.EntranceModule = Globales.sSerial;
                                            _Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;

                                            _frmPrincipal_Presenter.Escribirtarjeta();
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                if (Globales.sCarrilMixto == "SI")
                                {
                                    VehiculoMoto = false;
                                }
                                else
                                {
                                    VehiculoMoto = true;
                                }

                                #region Tarjetas
                                //if (_frmPrincipal_Presenter.ValidacionMotoMueble() == false && _frmPrincipal_Presenter.VehiculoMueble() == true)
                                //    {
                                //        VehiculoMoto = true;

                                //        //escribir
                                //        _Tarjeta.ActiveCycle = true;
                                //        _Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;
                                //        _Tarjeta.EntrancePlate = _sPlaca;
                                //        _Tarjeta.DateTimeEntrance = _FechaCompleta;
                                //        _Tarjeta.EntranceModule = Globales.sSerial;

                                //        _frmPrincipal_Presenter.Escribirtarjeta();
                                //    }
                                //    else
                                //    {
                                //        VehiculoMoto = false;

                                //        //escribir
                                //        _Tarjeta.ActiveCycle = true;
                                //        _Tarjeta.EntrancePlate = _sPlaca;
                                //        _Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;
                                //        _Tarjeta.DateTimeEntrance = _FechaCompleta;
                                //        _Tarjeta.EntranceModule = Globales.sSerial;

                                //        _frmPrincipal_Presenter.Escribirtarjeta();
                                //    }
                                #endregion

                                }

                        }
                    }
                        else
                        {
                            _frmPrincipal_Presenter.EstadoControl();
                            _BotonPresionado = false;
                            if (_VehiculoMueble)
                            {

                                General_Events = "(FrontEnd Antes CapturaPlaca)";
                                CapturaPlacaNew();
                                if (_frmPrincipal_Presenter.ValidarPlacaEntrada())
                                {
                                    if (_PlacaRegistrada == _sPlaca)
                                    {
                                        CapturaPlacaNew();
                                    }
                                    if (_sPlaca == _PlacaRegistrada)
                                    {
                                        _sPlaca = "------";
                                    }
                                }
                                General_Events = "(FrontEnd Antes CapturaPlaca) " + _sPlaca;


                                SoundPlayer simpleSound = new SoundPlayer(_sPathBienvenido);
                                simpleSound.Play();

                                SecuenciaTransaccion = DateTime.Now.ToString("yyyyMMddHHmmss") + Globales.sCarril + Globales.iCodigoEstacionamiento;

                                string ano = SecuenciaTransaccion.Substring(0, 4);
                                string mes = SecuenciaTransaccion.Substring(4, 2);
                                string dia = SecuenciaTransaccion.Substring(6, 2);
                                string hora = SecuenciaTransaccion.Substring(8, 2);
                                string min = SecuenciaTransaccion.Substring(10, 2);
                                string seg = SecuenciaTransaccion.Substring(12, 2);

                                string FechaCompleta = dia + "/" + mes + "/" + ano + " " + hora + ":" + min + ":" + seg;

                                _FechaCompleta = Convert.ToDateTime(FechaCompleta);

                        

                                if (_sPlaca != string.Empty && _sPlaca != "------")
                                {
                                    bool bAutorizado = false;
                                    bool ok2 = false;
                                    bool bAutoVencida2 = false;

                                    #region validacionAutorizado

                                    Autorizado oAutorizado = new Autorizado();
                                    oAutorizado.PlacaAuto = _sPlaca;
                                    oAutorizado.IdTarjeta = _IdCardAutorizado;
                                    General_Events = "oAutorizado.PlacaAuto : " + oAutorizado.PlacaAuto;

                                    if (_frmPrincipal_Presenter.ObtenerAutorizado(oAutorizado))
                                    {
                                        ResultadoOperacion oResultadoOperacion = new ResultadoOperacion();

                                        for (int i = 0; i < _lstDtoAutorizado.Count; i++)
                                        {
                                            if (_lstDtoAutorizado[i].IdEstacionamiento == Convert.ToInt64(Globales.iCodigoEstacionamiento))
                                            {

                                                if (_lstDtoAutorizado[i].EstadoAutorizacion && _lstDtoAutorizado[i].Estado && DateTime.Now >= _lstDtoAutorizado[i].FechaInicial && DateTime.Now <= _lstDtoAutorizado[i].FechaFinal)
                                                {
                                                if (_frmPrincipal_Presenter.ValidarIngresoAuto(_lstDtoAutorizado[i].IdTarjeta))
                                                {
                                                    ok2 = true;
                                                    bAutoVencida2 = false;
                                                    CntAuto = i;
                                                    break;
                                                }
                                                else
                                                {

                                                    #region OLD
                                                    //simpleSound = new SoundPlayer(_sPathTarjetaSinRegistroSalida);
                                                    //simpleSound.Play();
                                                    //Presentacion = Pantalla.TarjetaSinRegistroSalida;
                                                    //break;
                                                    ////General_Events = "TarjetaSinRegistroSalida";
                                                    //////ok = true;
                                                    //////bAutoVencida = false;
                                                    //////bTarjetaInvalida = false;
                                                    //////CntAuto = i;
                                                    //////break;
                                                    #endregion

                                                    simpleSound = new SoundPlayer(_sPathMensualidadUso);
                                                    simpleSound.Play();
                                                    _IdCardAutorizado = "USO";

                                                    if (Globales.sCarrilMixto == "NO")
                                                    {
                                                        VehiculoMoto = true;
                                                        #region Tarjeta
                                                        //escribir
                                                        //_Tarjeta.ActiveCycle = true;
                                                        //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;
                                                        //_Tarjeta.EntrancePlate = _sPlaca;
                                                        //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                        //_Tarjeta.EntranceModule = Globales.sSerial;

                                                        //_frmPrincipal_Presenter.Escribirtarjeta();
                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        VehiculoMoto = false;
                                                        #region Tarjeta
                                                        //escribir
                                                        //_Tarjeta.ActiveCycle = true;
                                                        //_Tarjeta.EntrancePlate = _sPlaca;
                                                        //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;
                                                        //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                        //_Tarjeta.EntranceModule = Globales.sSerial;

                                                        //_frmPrincipal_Presenter.Escribirtarjeta();
                                                        #endregion
                                                    }
                                                    //Presentacion = Pantalla.TarjetaSinRegistroSalida;
                                                    break;
                                                    ////General_Events = "TarjetaSinRegistroSalida";
                                                    ////ok = true;
                                                    ////bAutoVencida = false;
                                                    ////bTarjetaInvalida = false;
                                                    ////CntAuto = i;
                                                    ////break;
                                                }
                                        
                                                }
                                                else
                                                {
                                                    simpleSound = new SoundPlayer(_sPathAutoVencida);
                                                    simpleSound.Play();
                                                    bAutoVencida2 = true;
                                                    Presentacion = Pantalla.AutorizacionVencida;
                                                    if (_VehiculoMueble == false)
                                                    {
                                                        VehiculoMoto = true;
                                                        #region Tarjeta
                                                        //escribir
                                                        //_Tarjeta.ActiveCycle = true;
                                                        //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                        //_Tarjeta.EntranceModule = Globales.sSerial;
                                                        //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;

                                                        //_frmPrincipal_Presenter.Escribirtarjeta();
                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        VehiculoMoto = false;

                                                        #region Tarjeta
                                                        //escribir
                                                        //_Tarjeta.ActiveCycle = true;
                                                        //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                        //_Tarjeta.EntranceModule = Globales.sSerial;
                                                        //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;

                                                        //_frmPrincipal_Presenter.Escribirtarjeta();
                                                        #endregion
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                //Presentacion = Pantalla.TarjetaInvalida;
                                                //bTarjetaInvalida = true;
                                            }
                                        }

                                        if (ok2)
                                        {
                                            RegistroEntradaAutorizado(CntAuto);
                                        }
                                        //else if (bAutoVencida2)
                                        //{
                                        //    simpleSound = new SoundPlayer(_sPathAutoVencida);
                                        //    simpleSound.Play();
                                        //    Presentacion = Pantalla.AutorizacionVencida;
                                        //}
                                    }
                                    else
                                    {
                                        //_frmPrincipal_Presenter.EstadoControl();
                                        RegistroEntrada();
                                        TbTag.Text = string.Empty;
                                        //if (_VehiculoMueble==false)
                                        //{
                                        //    VehiculoMoto = true;

                                        //    //escribir
                                        //    _Tarjeta.ActiveCycle = true;
                                        //    _Tarjeta.EntrancePlate = _sPlaca;
                                        //    _Tarjeta.DateTimeEntrance = _FechaCompleta;
                                        //    _Tarjeta.EntranceModule = Globales.sSerial;
                                        //    _Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;

                                        //    _frmPrincipal_Presenter.Escribirtarjeta();
                                        //}
                                        //else
                                        //{
                                        //    VehiculoMoto = false;

                                        //    //escribir
                                        //    _Tarjeta.ActiveCycle = true;
                                        //    _Tarjeta.EntrancePlate = _sPlaca;
                                        //    _Tarjeta.DateTimeEntrance = _FechaCompleta;
                                        //    _Tarjeta.EntranceModule = Globales.sSerial;
                                        //    _Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;

                                        //    _frmPrincipal_Presenter.Escribirtarjeta();
                                        //}
                                    }
                                    #endregion
                                }
                                else if (_IdCardAutorizado != string.Empty)
                                {
                                    bool bAutorizado = false;
                                    bool ok2 = false;
                                    bool bAutoVencida2 = false;

                                    #region validacionAutorizado

                                    Autorizado oAutorizado = new Autorizado();
                                    oAutorizado.PlacaAuto = _sPlaca;
                                    oAutorizado.IdTarjeta = _IdCardAutorizado;
                                    General_Events = "oAutorizado.PlacaAuto : " + oAutorizado.PlacaAuto;

                                    if (_frmPrincipal_Presenter.ObtenerAutorizado(oAutorizado))
                                    {
                                        ResultadoOperacion oResultadoOperacion = new ResultadoOperacion();

                                        for (int i = 0; i < _lstDtoAutorizado.Count; i++)
                                        {
                                            if (_lstDtoAutorizado[i].IdEstacionamiento == Convert.ToInt64(Globales.iCodigoEstacionamiento))
                                            {

                                                if (_lstDtoAutorizado[i].EstadoAutorizacion && _lstDtoAutorizado[i].Estado && DateTime.Now >= _lstDtoAutorizado[i].FechaInicial && DateTime.Now <= _lstDtoAutorizado[i].FechaFinal)
                                                {
                                                    if (_frmPrincipal_Presenter.ValidarIngresoAuto(_lstDtoAutorizado[i].IdTarjeta))
                                                    {
                                                        ok2 = true;
                                                        bAutoVencida2 = false;
                                                        CntAuto = i;
                                                        break;
                                                    }
                                                    else
                                                    {

                                                        #region OLD
                                                        //simpleSound = new SoundPlayer(_sPathTarjetaSinRegistroSalida);
                                                        //simpleSound.Play();
                                                        //Presentacion = Pantalla.TarjetaSinRegistroSalida;
                                                        //break;
                                                        ////General_Events = "TarjetaSinRegistroSalida";
                                                        //////ok = true;
                                                        //////bAutoVencida = false;
                                                        //////bTarjetaInvalida = false;
                                                        //////CntAuto = i;
                                                        //////break;
                                                        #endregion

                                                        simpleSound = new SoundPlayer(_sPathMensualidadUso);
                                                        simpleSound.Play();
                                                        _IdCardAutorizado = "USO";

                                                        if (Globales.sCarrilMixto == "NO")
                                                        {
                                                            VehiculoMoto = true;
                                                            #region Tarjeta
                                                            //escribir
                                                            //_Tarjeta.ActiveCycle = true;
                                                            //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;
                                                            //_Tarjeta.EntrancePlate = _sPlaca;
                                                            //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                            //_Tarjeta.EntranceModule = Globales.sSerial;

                                                            //_frmPrincipal_Presenter.Escribirtarjeta();
                                                            #endregion
                                                        }
                                                        else
                                                        {
                                                            VehiculoMoto = false;
                                                            #region Tarjeta
                                                            //escribir
                                                            //_Tarjeta.ActiveCycle = true;
                                                            //_Tarjeta.EntrancePlate = _sPlaca;
                                                            //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;
                                                            //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                            //_Tarjeta.EntranceModule = Globales.sSerial;

                                                            //_frmPrincipal_Presenter.Escribirtarjeta();
                                                            #endregion
                                                        }
                                                        //Presentacion = Pantalla.TarjetaSinRegistroSalida;
                                                        break;
                                                        ////General_Events = "TarjetaSinRegistroSalida";
                                                        ////ok = true;
                                                        ////bAutoVencida = false;
                                                        ////bTarjetaInvalida = false;
                                                        ////CntAuto = i;
                                                        ////break;
                                                    }

                                                }
                                                else
                                                {
                                                    simpleSound = new SoundPlayer(_sPathTarjetaInvalida);
                                                    simpleSound.Play();
                                                    bAutoVencida2 = true;
                                                    Presentacion = Pantalla.SalvaPantallas;
                                                    TbTag.Text = "";
                                                    _IdCardAutorizado = "";
                                                    if (_VehiculoMueble == false)
                                                    {
                                                        VehiculoMoto = true;
                                                        #region Tarjeta
                                                        //escribir
                                                        //_Tarjeta.ActiveCycle = true;
                                                        //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                        //_Tarjeta.EntranceModule = Globales.sSerial;
                                                        //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;

                                                        //_frmPrincipal_Presenter.Escribirtarjeta();
                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        VehiculoMoto = false;

                                                        #region Tarjeta
                                                        //escribir
                                                        //_Tarjeta.ActiveCycle = true;
                                                        //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                        //_Tarjeta.EntranceModule = Globales.sSerial;
                                                        //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;

                                                        //_frmPrincipal_Presenter.Escribirtarjeta();
                                                        #endregion
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                //Presentacion = Pantalla.TarjetaInvalida;
                                                //bTarjetaInvalida = true;
                                            }
                                        }

                                        if (ok2)
                                        {
                                            RegistroEntradaAutorizado(CntAuto);
                                        }
                                        //else if (bAutoVencida2)
                                        //{
                                        //    simpleSound = new SoundPlayer(_sPathAutoVencida);
                                        //    simpleSound.Play();
                                        //    Presentacion = Pantalla.AutorizacionVencida;
                                        //}
                                    }
                                    else
                                    {
                                        //_frmPrincipal_Presenter.EstadoControl();
                                        RegistroEntrada();
                                        TbTag.Text = "";
                                        _IdCardAutorizado = "";
                                        //if (_VehiculoMueble==false)
                                        //{
                                        //    VehiculoMoto = true;

                                        //    //escribir
                                        //    _Tarjeta.ActiveCycle = true;
                                        //    _Tarjeta.EntrancePlate = _sPlaca;
                                        //    _Tarjeta.DateTimeEntrance = _FechaCompleta;
                                        //    _Tarjeta.EntranceModule = Globales.sSerial;
                                        //    _Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;

                                        //    _frmPrincipal_Presenter.Escribirtarjeta();
                                        //}
                                        //else
                                        //{
                                        //    VehiculoMoto = false;

                                        //    //escribir
                                        //    _Tarjeta.ActiveCycle = true;
                                        //    _Tarjeta.EntrancePlate = _sPlaca;
                                        //    _Tarjeta.DateTimeEntrance = _FechaCompleta;
                                        //    _Tarjeta.EntranceModule = Globales.sSerial;
                                        //    _Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;

                                        //    _frmPrincipal_Presenter.Escribirtarjeta();
                                        //}
                                    }
                                    #endregion
                                }

                                else
                                {

                                    if (Globales.sCarrilMixto == "NO")
                                    {

                                        VehiculoMoto = true;

                                        #region Tarjeta
                                        ////escribir
                                        //_Tarjeta.ActiveCycle = true;
                                        //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;
                                        //_Tarjeta.EntrancePlate = _sPlaca;
                                        //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                        //_Tarjeta.EntranceModule = Globales.sSerial;
                                        //General_Events = "(FrontEnd Antes EscribirTarjetaNew)";
                                        //_frmPrincipal_Presenter.Escribirtarjeta();
                                        //General_Events = "(FrontEnd Despues EscribirTarjetaNew)";
                                        #endregion
                                        Presentacion = Pantalla.RetireTarjeta;
                                    }
                                    else
                                    {
                                        VehiculoMoto = false;
                                        #region Tarjeta     
                                        ////escribir
                                        //_Tarjeta.ActiveCycle = true;
                                        //_Tarjeta.EntrancePlate = _sPlaca;
                                        //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;
                                        //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                        //_Tarjeta.EntranceModule = Globales.sSerial;
                                        //General_Events = "(FrontEnd Antes EscribirTarjetaNew)";
                                        //_frmPrincipal_Presenter.Escribirtarjeta();
                                        //General_Events = "(FrontEnd Despues EscribirTarjetaNew)";
                                        #endregion

                                        Presentacion = Pantalla.RetireTarjeta;
                                    }
                                }
                            }
                                
                            else if (_sPlaca != string.Empty && _sPlaca != "------")
                            {
                               
                                #region validacionAutorizado

                                Autorizado oAutorizado = new Autorizado();
                                oAutorizado.PlacaAuto = _sPlaca;
                                oAutorizado.IdTarjeta = _IdCardAutorizado;
                                General_Events = "oAutorizado.PlacaAuto : " + oAutorizado.PlacaAuto;

                                if (_frmPrincipal_Presenter.ObtenerAutorizado(oAutorizado))
                                {

                                    SoundPlayer simpleSoundNew = new SoundPlayer(_sPathBienvenido);
                                    simpleSoundNew.Play();

                                    SecuenciaTransaccion = DateTime.Now.ToString("yyyyMMddHHmmss") + Globales.sCarril + Globales.iCodigoEstacionamiento;

                                    string ano = SecuenciaTransaccion.Substring(0, 4);
                                    string mes = SecuenciaTransaccion.Substring(4, 2);
                                    string dia = SecuenciaTransaccion.Substring(6, 2);
                                    string hora = SecuenciaTransaccion.Substring(8, 2);
                                    string min = SecuenciaTransaccion.Substring(10, 2);
                                    string seg = SecuenciaTransaccion.Substring(12, 2);

                                    string FechaCompleta = dia + "/" + mes + "/" + ano + " " + hora + ":" + min + ":" + seg;

                                    _FechaCompleta = Convert.ToDateTime(FechaCompleta);

                                    bool bAutorizado = false;
                                    bool ok2 = false;
                                    bool bAutoVencida2 = false;
                                    ResultadoOperacion oResultadoOperacion = new ResultadoOperacion();

                                    for (int i = 0; i < _lstDtoAutorizado.Count; i++)
                                    {
                                        if (_lstDtoAutorizado[i].IdEstacionamiento == Convert.ToInt64(Globales.iCodigoEstacionamiento))
                                        {

                                            if (_lstDtoAutorizado[i].EstadoAutorizacion && _lstDtoAutorizado[i].Estado && DateTime.Now >= _lstDtoAutorizado[i].FechaInicial && DateTime.Now <= _lstDtoAutorizado[i].FechaFinal)
                                            {
                                                if (_frmPrincipal_Presenter.ValidarIngresoAuto(_lstDtoAutorizado[i].IdTarjeta))
                                                {
                                                    ok2 = true;
                                                    bAutoVencida2 = false;
                                                    CntAuto = i;
                                                    break;
                                                }
                                                else
                                                {

                                                    #region OLD
                                                    //simpleSound = new SoundPlayer(_sPathTarjetaSinRegistroSalida);
                                                    //simpleSound.Play();
                                                    //Presentacion = Pantalla.TarjetaSinRegistroSalida;
                                                    //break;
                                                    ////General_Events = "TarjetaSinRegistroSalida";
                                                    //////ok = true;
                                                    //////bAutoVencida = false;
                                                    //////bTarjetaInvalida = false;
                                                    //////CntAuto = i;
                                                    //////break;
                                                    #endregion

                                                    simpleSoundNew = new SoundPlayer(_sPathMensualidadUso);
                                                    simpleSoundNew.Play();
                                                    _IdCardAutorizado = "USO";

                                                    if (Globales.sCarrilMixto == "NO")
                                                    {
                                                        VehiculoMoto = true;
                                                        #region Tarjeta
                                                        //escribir
                                                        //_Tarjeta.ActiveCycle = true;
                                                        //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;
                                                        //_Tarjeta.EntrancePlate = _sPlaca;
                                                        //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                        //_Tarjeta.EntranceModule = Globales.sSerial;

                                                        //_frmPrincipal_Presenter.Escribirtarjeta();
                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        VehiculoMoto = false;
                                                        #region Tarjeta
                                                        //escribir
                                                        //_Tarjeta.ActiveCycle = true;
                                                        //_Tarjeta.EntrancePlate = _sPlaca;
                                                        //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;
                                                        //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                        //_Tarjeta.EntranceModule = Globales.sSerial;

                                                        //_frmPrincipal_Presenter.Escribirtarjeta();
                                                        #endregion
                                                    }
                                                    Presentacion = Pantalla.RetireTarjeta;
                                                    TbTag.Text = "";
                                                    _IdCardAutorizado = "";
                                                    //Presentacion = Pantalla.TarjetaSinRegistroSalida;
                                                    break;
                                                    ////General_Events = "TarjetaSinRegistroSalida";
                                                    ////ok = true;
                                                    ////bAutoVencida = false;
                                                    ////bTarjetaInvalida = false;
                                                    ////CntAuto = i;
                                                    ////break;
                                                }

                                            }
                                            else
                                            {
                                                simpleSoundNew = new SoundPlayer(_sPathAutoVencida);
                                                simpleSoundNew.Play();
                                                bAutoVencida2 = true;
                                                TbTag.Text = "";
                                                IdCardAutorizado = "";
                                                Presentacion = Pantalla.AutorizacionVencida;
                                                if (_VehiculoMueble == false)
                                                {
                                                    VehiculoMoto = true;
                                                    #region Tarjeta
                                                    //escribir
                                                    //_Tarjeta.ActiveCycle = true;
                                                    //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                    //_Tarjeta.EntranceModule = Globales.sSerial;
                                                    //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;

                                                    //_frmPrincipal_Presenter.Escribirtarjeta();
                                                    #endregion
                                                }
                                                else
                                                {
                                                    VehiculoMoto = false;

                                                    #region Tarjeta
                                                    //escribir
                                                    //_Tarjeta.ActiveCycle = true;
                                                    //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                    //_Tarjeta.EntranceModule = Globales.sSerial;
                                                    //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;

                                                    //_frmPrincipal_Presenter.Escribirtarjeta();
                                                    #endregion
                                                }

                                            }
                                        }
                                        else
                                        {
                                            //Presentacion = Pantalla.TarjetaInvalida;
                                            //bTarjetaInvalida = true;
                                        }
                                    }

                                    if (ok2)
                                    {
                                        RegistroEntradaAutorizado(CntAuto);
                                        TbTag.Text = "";
                                        _IdCardAutorizado = "";
                                    }
                                    //else if (bAutoVencida2)
                                    //{
                                    //    simpleSound = new SoundPlayer(_sPathAutoVencida);
                                    //    simpleSound.Play();
                                    //    Presentacion = Pantalla.AutorizacionVencida;
                                    //}
                                }
                                else
                                {
                                    #region OldElse
                                    ////_frmPrincipal_Presenter.EstadoControl();
                                    ////RegistroEntrada();
                                    //TbTag.Text = "";
                                    //_IdCardAutorizado = "";
                                    //Presentacion = Pantalla.SalvaPantallas;

                                    ////if (_VehiculoMueble==false)
                                    ////{
                                    ////    VehiculoMoto = true;

                                    ////    //escribir
                                    ////    _Tarjeta.ActiveCycle = true;
                                    ////    _Tarjeta.EntrancePlate = _sPlaca;
                                    ////    _Tarjeta.DateTimeEntrance = _FechaCompleta;
                                    ////    _Tarjeta.EntranceModule = Globales.sSerial;
                                    ////    _Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;

                                    ////    _frmPrincipal_Presenter.Escribirtarjeta();
                                    ////}
                                    ////else
                                    ////{
                                    ////    VehiculoMoto = false;

                                    ////    //escribir
                                    ////    _Tarjeta.ActiveCycle = true;
                                    ////    _Tarjeta.EntrancePlate = _sPlaca;
                                    ////    _Tarjeta.DateTimeEntrance = _FechaCompleta;
                                    ////    _Tarjeta.EntranceModule = Globales.sSerial;
                                    ////    _Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;

                                    ////    _frmPrincipal_Presenter.Escribirtarjeta();
                                    ////}
                                    #endregion

                              if (_IdCardAutorizado != string.Empty)
                                {
                                SoundPlayer simpleSound = new SoundPlayer(_sPathBienvenido);
                                simpleSound.Play();

                                SecuenciaTransaccion = DateTime.Now.ToString("yyyyMMddHHmmss") + Globales.sCarril + Globales.iCodigoEstacionamiento;

                                string ano = SecuenciaTransaccion.Substring(0, 4);
                                string mes = SecuenciaTransaccion.Substring(4, 2);
                                string dia = SecuenciaTransaccion.Substring(6, 2);
                                string hora = SecuenciaTransaccion.Substring(8, 2);
                                string min = SecuenciaTransaccion.Substring(10, 2);
                                string seg = SecuenciaTransaccion.Substring(12, 2);

                                string FechaCompleta = dia + "/" + mes + "/" + ano + " " + hora + ":" + min + ":" + seg;

                                _FechaCompleta = Convert.ToDateTime(FechaCompleta);
                                bool bAutorizado = false;
                                bool ok2 = false;
                                bool bAutoVencida2 = false;

                                #region validacionAutorizado

                                Autorizado oAutorizadoN = new Autorizado();
                                oAutorizadoN.PlacaAuto = _sPlaca;
                                oAutorizadoN.IdTarjeta = _IdCardAutorizado;
                                General_Events = "oAutorizado.PlacaAuto : " + oAutorizadoN.PlacaAuto;

                                if (_frmPrincipal_Presenter.ObtenerAutorizado(oAutorizado))
                                {
                                    ResultadoOperacion oResultadoOperacion = new ResultadoOperacion();

                                    for (int i = 0; i < _lstDtoAutorizado.Count; i++)
                                    {
                                        if (_lstDtoAutorizado[i].IdEstacionamiento == Convert.ToInt64(Globales.iCodigoEstacionamiento))
                                        {

                                            if (_lstDtoAutorizado[i].EstadoAutorizacion && _lstDtoAutorizado[i].Estado && DateTime.Now >= _lstDtoAutorizado[i].FechaInicial && DateTime.Now <= _lstDtoAutorizado[i].FechaFinal)
                                            {
                                                if (_frmPrincipal_Presenter.ValidarIngresoAuto(_lstDtoAutorizado[i].IdTarjeta))
                                                {
                                                    ok2 = true;
                                                    bAutoVencida2 = false;
                                                    CntAuto = i;
                                                    break;
                                                }
                                                else
                                                {
                                                    SoundPlayer simpleSoundNew = new SoundPlayer(_sPathMensualidadUso);
                                                    simpleSoundNew.Play();
                                                    _IdCardAutorizado = "USO";

                                                    if (Globales.sCarrilMixto == "NO")
                                                    {
                                                        VehiculoMoto = true;
                                                        #region Tarjeta
                                                        //escribir
                                                        //_Tarjeta.ActiveCycle = true;
                                                        //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;
                                                        //_Tarjeta.EntrancePlate = _sPlaca;
                                                        //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                        //_Tarjeta.EntranceModule = Globales.sSerial;

                                                        //_frmPrincipal_Presenter.Escribirtarjeta();
                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        VehiculoMoto = false;
                                                        #region Tarjeta
                                                        //escribir
                                                        //_Tarjeta.ActiveCycle = true;
                                                        //_Tarjeta.EntrancePlate = _sPlaca;
                                                        //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;
                                                        //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                        //_Tarjeta.EntranceModule = Globales.sSerial;

                                                        //_frmPrincipal_Presenter.Escribirtarjeta();
                                                        #endregion
                                                    }
                                                    Presentacion = Pantalla.SalvaPantallas;
                                                    TbTag.Text = "";
                                                    _IdCardAutorizado = "";
                                                    //Presentacion = Pantalla.TarjetaSinRegistroSalida;
                                                    break;
                                                    ////General_Events = "TarjetaSinRegistroSalida";
                                                    ////ok = true;
                                                    ////bAutoVencida = false;
                                                    ////bTarjetaInvalida = false;
                                                    ////CntAuto = i;
                                                    ////break;
                                                }

                                            }
                                            else
                                            {
                                                SoundPlayer simpleSoundNew = new SoundPlayer(_sPathAutoVencida);
                                                simpleSoundNew.Play();
                                                bAutoVencida2 = true;
                                                TbTag.Text = "";
                                                IdCardAutorizado = "";
                                                Presentacion = Pantalla.AutorizacionVencida;
                                                if (_VehiculoMueble == false)
                                                {
                                                    VehiculoMoto = true;
                                                    #region Tarjeta
                                                    //escribir
                                                    //_Tarjeta.ActiveCycle = true;
                                                    //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                    //_Tarjeta.EntranceModule = Globales.sSerial;
                                                    //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;

                                                    //_frmPrincipal_Presenter.Escribirtarjeta();
                                                    #endregion
                                                }
                                                else
                                                {
                                                    VehiculoMoto = false;

                                                    #region Tarjeta
                                                    //escribir
                                                    //_Tarjeta.ActiveCycle = true;
                                                    //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                    //_Tarjeta.EntranceModule = Globales.sSerial;
                                                    //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;

                                                    //_frmPrincipal_Presenter.Escribirtarjeta();
                                                    #endregion
                                                }

                                            }
                                        }
                                        else
                                        {
                                            //Presentacion = Pantalla.TarjetaInvalida;
                                            //bTarjetaInvalida = true;
                                        }
                                    }

                                    if (ok2)
                                    {
                                        TbTag.Text = string.Empty;
                                        _IdCardAutorizado = "";
                                        RegistroEntradaAutorizado(CntAuto);


                                    }
                                    //else if (bAutoVencida2)
                                    //{
                                    //    simpleSound = new SoundPlayer(_sPathAutoVencida);
                                    //    simpleSound.Play();
                                    //    Presentacion = Pantalla.AutorizacionVencida;
                                    //}
                                }
                                else
                                {
                                    //_frmPrincipal_Presenter.EstadoControl();
                                    Presentacion = Pantalla.RetireTarjeta;
                                    TbTag.Text = "";
                                    _IdCardAutorizado = "";
                                    //if (_VehiculoMueble==false)
                                    //{
                                    //    VehiculoMoto = true;

                                    //    //escribir
                                    //    _Tarjeta.ActiveCycle = true;
                                    //    _Tarjeta.EntrancePlate = _sPlaca;
                                    //    _Tarjeta.DateTimeEntrance = _FechaCompleta;
                                    //    _Tarjeta.EntranceModule = Globales.sSerial;
                                    //    _Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;

                                    //    _frmPrincipal_Presenter.Escribirtarjeta();
                                    //}
                                    //else
                                    //{
                                    //    VehiculoMoto = false;

                                    //    //escribir
                                    //    _Tarjeta.ActiveCycle = true;
                                    //    _Tarjeta.EntrancePlate = _sPlaca;
                                    //    _Tarjeta.DateTimeEntrance = _FechaCompleta;
                                    //    _Tarjeta.EntranceModule = Globales.sSerial;
                                    //    _Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;

                                    //    _frmPrincipal_Presenter.Escribirtarjeta();
                                    //}
                                }
                                #endregion
                            }


                                }
                                #endregion
                            }
                            else if (_IdCardAutorizado != string.Empty)
                            {
                                SoundPlayer simpleSound = new SoundPlayer(_sPathBienvenido);
                                simpleSound.Play();

                                SecuenciaTransaccion = DateTime.Now.ToString("yyyyMMddHHmmss") + Globales.sCarril + Globales.iCodigoEstacionamiento;

                                string ano = SecuenciaTransaccion.Substring(0, 4);
                                string mes = SecuenciaTransaccion.Substring(4, 2);
                                string dia = SecuenciaTransaccion.Substring(6, 2);
                                string hora = SecuenciaTransaccion.Substring(8, 2);
                                string min = SecuenciaTransaccion.Substring(10, 2);
                                string seg = SecuenciaTransaccion.Substring(12, 2);

                                string FechaCompleta = dia + "/" + mes + "/" + ano + " " + hora + ":" + min + ":" + seg;

                                _FechaCompleta = Convert.ToDateTime(FechaCompleta);
                                bool bAutorizado = false;
                                bool ok2 = false;
                                bool bAutoVencida2 = false;

                                #region validacionAutorizado

                                Autorizado oAutorizado = new Autorizado();
                                oAutorizado.PlacaAuto = _sPlaca;
                                oAutorizado.IdTarjeta = _IdCardAutorizado;
                                General_Events = "oAutorizado.PlacaAuto : " + oAutorizado.PlacaAuto;

                                if (_frmPrincipal_Presenter.ObtenerAutorizado(oAutorizado))
                                {
                                    ResultadoOperacion oResultadoOperacion = new ResultadoOperacion();

                                    for (int i = 0; i < _lstDtoAutorizado.Count; i++)
                                    {
                                        if (_lstDtoAutorizado[i].IdEstacionamiento == Convert.ToInt64(Globales.iCodigoEstacionamiento))
                                        {

                                            if (_lstDtoAutorizado[i].EstadoAutorizacion && _lstDtoAutorizado[i].Estado && DateTime.Now >= _lstDtoAutorizado[i].FechaInicial && DateTime.Now <= _lstDtoAutorizado[i].FechaFinal)
                                            {
                                                if (_frmPrincipal_Presenter.ValidarIngresoAuto(_lstDtoAutorizado[i].IdTarjeta))
                                                {
                                                    ok2 = true;
                                                    bAutoVencida2 = false;
                                                    CntAuto = i;
                                                    break;
                                                }
                                                else
                                                {

                                                    #region OLD
                                                    //simpleSound = new SoundPlayer(_sPathTarjetaSinRegistroSalida);
                                                    //simpleSound.Play();
                                                    //Presentacion = Pantalla.TarjetaSinRegistroSalida;
                                                    //break;
                                                    ////General_Events = "TarjetaSinRegistroSalida";
                                                    //////ok = true;
                                                    //////bAutoVencida = false;
                                                    //////bTarjetaInvalida = false;
                                                    //////CntAuto = i;
                                                    //////break;
                                                    #endregion

                                                    SoundPlayer simpleSoundNew = new SoundPlayer(_sPathMensualidadUso);
                                                    simpleSoundNew.Play();
                                                    _IdCardAutorizado = "USO";

                                                    if (Globales.sCarrilMixto == "NO")
                                                    {
                                                        VehiculoMoto = true;
                                                        #region Tarjeta
                                                        //escribir
                                                        //_Tarjeta.ActiveCycle = true;
                                                        //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;
                                                        //_Tarjeta.EntrancePlate = _sPlaca;
                                                        //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                        //_Tarjeta.EntranceModule = Globales.sSerial;

                                                        //_frmPrincipal_Presenter.Escribirtarjeta();
                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        VehiculoMoto = false;
                                                        #region Tarjeta
                                                        //escribir
                                                        //_Tarjeta.ActiveCycle = true;
                                                        //_Tarjeta.EntrancePlate = _sPlaca;
                                                        //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;
                                                        //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                        //_Tarjeta.EntranceModule = Globales.sSerial;

                                                        //_frmPrincipal_Presenter.Escribirtarjeta();
                                                        #endregion
                                                    }
                                                    Presentacion = Pantalla.SalvaPantallas;
                                                    TbTag.Text = "";
                                                    _IdCardAutorizado = "";
                                                    //Presentacion = Pantalla.TarjetaSinRegistroSalida;
                                                    break;
                                                    ////General_Events = "TarjetaSinRegistroSalida";
                                                    ////ok = true;
                                                    ////bAutoVencida = false;
                                                    ////bTarjetaInvalida = false;
                                                    ////CntAuto = i;
                                                    ////break;
                                                }

                                            }
                                            else
                                            {
                                                SoundPlayer simpleSoundNew = new SoundPlayer(_sPathAutoVencida);
                                                simpleSoundNew.Play();
                                                bAutoVencida2 = true;
                                                TbTag.Text = "";
                                                IdCardAutorizado = "";
                                                Presentacion = Pantalla.AutorizacionVencida;
                                                if (_VehiculoMueble == false)
                                                {
                                                    VehiculoMoto = true;
                                                    #region Tarjeta
                                                    //escribir
                                                    //_Tarjeta.ActiveCycle = true;
                                                    //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                    //_Tarjeta.EntranceModule = Globales.sSerial;
                                                    //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;

                                                    //_frmPrincipal_Presenter.Escribirtarjeta();
                                                    #endregion
                                                }
                                                else
                                                {
                                                    VehiculoMoto = false;

                                                    #region Tarjeta
                                                    //escribir
                                                    //_Tarjeta.ActiveCycle = true;
                                                    //_Tarjeta.DateTimeEntrance = _FechaCompleta;
                                                    //_Tarjeta.EntranceModule = Globales.sSerial;
                                                    //_Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;

                                                    //_frmPrincipal_Presenter.Escribirtarjeta();
                                                    #endregion
                                                }

                                            }
                                        }
                                        else
                                        {
                                            //Presentacion = Pantalla.TarjetaInvalida;
                                            //bTarjetaInvalida = true;
                                        }
                                    }

                                    if (ok2)
                                    {
                                        TbTag.Text = string.Empty;
                                        _IdCardAutorizado = "";
                                        RegistroEntradaAutorizado(CntAuto);


                                    }
                                    //else if (bAutoVencida2)
                                    //{
                                    //    simpleSound = new SoundPlayer(_sPathAutoVencida);
                                    //    simpleSound.Play();
                                    //    Presentacion = Pantalla.AutorizacionVencida;
                                    //}
                                }
                                else
                                {
                                    //_frmPrincipal_Presenter.EstadoControl();
                                    Presentacion = Pantalla.RetireTarjeta;
                                    TbTag.Text = "";
                                    _IdCardAutorizado = "";
                                    //if (_VehiculoMueble==false)
                                    //{
                                    //    VehiculoMoto = true;

                                    //    //escribir
                                    //    _Tarjeta.ActiveCycle = true;
                                    //    _Tarjeta.EntrancePlate = _sPlaca;
                                    //    _Tarjeta.DateTimeEntrance = _FechaCompleta;
                                    //    _Tarjeta.EntranceModule = Globales.sSerial;
                                    //    _Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.MOTORCYCLE;

                                    //    _frmPrincipal_Presenter.Escribirtarjeta();
                                    //}
                                    //else
                                    //{
                                    //    VehiculoMoto = false;

                                    //    //escribir
                                    //    _Tarjeta.ActiveCycle = true;
                                    //    _Tarjeta.EntrancePlate = _sPlaca;
                                    //    _Tarjeta.DateTimeEntrance = _FechaCompleta;
                                    //    _Tarjeta.EntranceModule = Globales.sSerial;
                                    //    _Tarjeta.TypeVehicle = TYPEVEHICLE_TARJETAPARKING_V1.AUTOMOBILE;

                                    //    _frmPrincipal_Presenter.Escribirtarjeta();
                                    //}
                                }
                                #endregion
                            }
                   
                        }

                        if (_frmPrincipal_Presenter.ObtenerEventoDispo())
                        {
                            string[] Resul = _Barrera.Split(';');

                            if (Resul[0].ToString() == Globales.sSerial)
                            {
                                if (Convert.ToBoolean(Globales.sPLC) == true)
                                {
                                    _frmPrincipal_Presenter.AbrirTalanquera();
                                }
                                else
                                {
                                    _frmPrincipal_Presenter.AperturaBarrera();
                                }
                            }

                            _frmPrincipal_Presenter.ActualizarEventoDispo(Convert.ToInt64(Resul[1]));
                        }
                    //}
                    //}
                    break;

                case Pantalla.RetireTarjeta:

                    #region Tarjeta
                    ////_frmPrincipal_Presenter.PocisionCard();
                    ////_frmPrincipal_Presenter.GetIdCard();
                    //_frmPrincipal_Presenter.ReadCard();
                    //_frmPrincipal_Presenter.StateDispenserNew();
#endregion

                    bool bAutoVencida = false;
                    bool bTarjetaInvalida = false;
                    bool ok = false;

                        if (VehiculoMoto)
                        {
                            oTransaccion.TipoVehiculo = 2;
                        }
                        else
                        {
                            oTransaccion.TipoVehiculo = 1;
                        }

                        RegistroEntrada();                 
                    break;

                case Pantalla.DisfruteVisita:
                    if (Convert.ToBoolean(Globales.sPLC) == true)
                    {
                        if (_frmPrincipal_Presenter.VehiculoTalanquera())
                        {
                            bool sound = false;
                            //_frmPrincipal_Presenter.AlistarTarjeta();
                            if (!sound)
                            {
                                SoundPlayer simpleSound = new SoundPlayer(_sPathVehiculoEntrando);
                                simpleSound.Play();
                                sound = true;
                            }
                            //EnvioImagenes();
                            Presentacion = Pantalla.VehiculoEntrando;
                            TbTag.Focus();
                        }
                    }
                    else
                    {
                        _frmPrincipal_Presenter.EstadoControl();
                        if (_VehiculoTalanquera == true && _VehiculoMueble==false)
                        {
                            bool sound = false;
                            //_frmPrincipal_Presenter.AlistarTarjeta();
                            if (!sound)
                            {
                                SoundPlayer simpleSound = new SoundPlayer(_sPathVehiculoEntrando);
                                simpleSound.Play();
                                sound = true;
                            }
                            //EnvioImagenes();
                            _cnt_timeout = 0;
                            Presentacion = Pantalla.VehiculoEntrando;
                        }
                    }
                    break;
                case Pantalla.DisfruteVisitaAuto:
                    if (Convert.ToBoolean(Globales.sPLC) == true)
                    {
                        if (_frmPrincipal_Presenter.VehiculoTalanquera())
                        {
                            bool sound = false;

                            if (!sound)
                            {
                                SoundPlayer simpleSound = new SoundPlayer(_sPathVehiculoEntrando);
                                simpleSound.Play();
                                sound = true;
                            }
                            //EnvioImagenes();
                            Presentacion = Pantalla.VehiculoEntrando;
                            
                        }
                    }
                    else
                    {
                        _frmPrincipal_Presenter.EstadoControl();
                        if (_VehiculoMueble == false)
                        {
                                bool sound = false;
                                if (!sound)
                                {
                                    SoundPlayer simpleSound = new SoundPlayer(_sPathVehiculoEntrando);
                                    simpleSound.Play();
                                    sound = true;
                                }
                                Presentacion = Pantalla.VehiculoEntrando;
                                _IdCardAutorizado = IdCardAutorizado;
                        }
                    }
                    break;
                case Pantalla.VehiculoEntrando:
                    if (Convert.ToBoolean(Globales.sPLC) == true)
                    {
                        if (_frmPrincipal_Presenter.VehiculoSalioTalanquera())
                        {

                            if (_frmPrincipal_Presenter.LimpiarValoresPLC())
                            {
                                Presentacion = Pantalla.SalvaPantallas;
                            }
                        }
                    }
                    else
                    {
                        _frmPrincipal_Presenter.EstadoControl();
                        if (_VehiculoTalanquera == false && _VehiculoMueble==false)
                        {
                            Presentacion = Pantalla.SalvaPantallas;
                        }
                        //if (cnt_timeout == (int)TimeOut.TimeOut_Alertas)
                        //{
                        //    Thread.Sleep(1300);
                        //   Presentacion = Pantalla.SalvaPantallas;
                        //    //VehiculoTalanquera = false;
                        //    _IdCardAutorizado = IdCardAutorizado;
                        //}
                    }
                    break;

                case Pantalla.TarjetaInvalida:
                    if (cnt_timeout == (int)TimeOut.TimeOut_Alertas)
                    {
                        Presentacion = Pantalla.SalvaPantallas;
                    }
                    break;

                case Pantalla.AutorizacionVencida:
                    if (Convert.ToBoolean(Globales.sPLC) == true)
                    {
                        _frmPrincipal_Presenter.ReadCard();
                        _frmPrincipal_Presenter.StateDispenserNew();

                        if (_RemoveCard)
                        {
                            RegistroEntrada();
                        }

                        if (_frmPrincipal_Presenter.VehiculoMueble() == false)
                        {
                            _frmPrincipal_Presenter.DevolverTarjetaNew();
                            Presentacion = Pantalla.SalvaPantallas;
                        }
                    }
                    else
                    {
                        if (cnt_timeout == (int)TimeOut.TimeOut_MensualidadVencida)
                        {
                            _frmPrincipal_Presenter.EstadoControl();
                            if (_VehiculoMueble)
                            {
                                Presentacion = Pantalla.RetireTarjeta;
                            }
                            else
                            {
                                Presentacion = Pantalla.SalvaPantallas;
                            }
                        }
                    }
                 

                    break;

                case Pantalla.TarjetaSinRegistroSalida:
                    if (cnt_timeout == (int)TimeOut.TimeOut_Alertas)
                    {
                        Presentacion = Pantalla.SalvaPantallas;
                    }
                    break;
                case Pantalla.SinTarjetas:
                    _frmPrincipal_Presenter.StateDispenser();
                    if (SsinTar == 45)
                    {
                        soundSinTar = false;
                        SsinTar = 0;
                    }
                    break;
            }
        }
        private void tmrEnvioImagenes_Tick(object sender, EventArgs e)
        {
            EnvioImagenes();
        }
        private void TbTag_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)13)
            {

                if (TbTag.Text != string.Empty && TbTag.Text.Length <= 17)
                {
                    string IdCarAutorizadoNew = TbTag.Text.Trim();

                    if (IdCarAutorizadoNew == _IdCardAutorizado)
                    {
                        _IdCardAutorizado = TbTag.Text.Trim();
                        
                    }
                    else
                    {
                        _IdCardAutorizado = IdCarAutorizadoNew;
                    }

                    
                }
                else
                {
                    TbTag.Text = string.Empty;
                }
            }
        }
        #endregion

        #region General
        private async Task Inicio()
        {
            if (Globales.sTestMode != string.Empty)
            {
                if (Convert.ToBoolean(Globales.sTestMode) != true)
                {
                    Cursor.Hide();
                    this.TopMost = true;

                }
                else
                {
                    Cursor.Show();
                    this.TopMost = false;
                }
            }
            else
            {
                Cursor.Hide();
                this.TopMost = true;
            }


            TraceHandler.WriteLine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Logs\Log"), "INICIO REGISTRO", TipoLog.TRAZA);

            if (CargaRecursos())
            {
                if (CargaImagenes())
                {
                    if (CargaSonidos())
                    {
                        if (CargaAnimaciones())
                        {
                            if (Globales.sSerial != string.Empty)
                            {
                                if (CargarParametros())
                                {

                                    _frmPrincipal_Presenter.SolucionarTodasAlarmas();

                                    var th1 = await ConectarDispositivos();

                                    if (th1)
                                    {
                                        //CapturaPlaca();
                                        Presentacion = Pantalla.SalvaPantallas;
                                    }
                                    else
                                    {
                                        //MessageBox.Show("SISTEMA SUSPENDIDO");
                                        Presentacion = Pantalla.SistemaSuspendido;
                                    }
                                }
                                else
                                {
                                    InfoModulo = true;
                                    Presentacion = Pantalla.SistemaSuspendido;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Antes de iniciar el punto de pago debe configurar el nombre del modulo.");
                                Presentacion = Pantalla.SistemaSuspendido;
                            }
                        }
                        else
                        {
                            //Presentacion = Pantalla.SistemaSuspendido;
                        }
                    }

                }

            }

        }
        private bool CargarParametros()
        {
            bool ok = false;

            if (_frmPrincipal_Presenter.ObtenerInfoModulo())
            {
                if (_frmPrincipal_Presenter.ObtenerPartes())
                {
                    if (_frmPrincipal_Presenter.ObtenerParametros())
                    {
                        ok = true;
                    }
                }
            }

            return ok;
        }
        private bool CargaRecursos()
        {
            bool ok = false;

            try
            {
                lblDatosAuto.BackColor = Color.Transparent;
                lblDatosAuto.Text = string.Empty;

                lblBienvenido.BackColor = Color.Transparent;


                Animacion_Principal.Dock = DockStyle.Fill;
                Animacion_Principal.SizeMode = PictureBoxSizeMode.StretchImage;
                Animacion_PublicidadSecundaria.Size = new System.Drawing.Size(1024, 350);
                Animacion_PublicidadSecundaria.Location = new Point(0, 0);
                Animacion_PublicidadSecundaria.SizeMode = PictureBoxSizeMode.StretchImage;
                Animacion_RetireTarjeta.Size = new System.Drawing.Size(1024, 400);
                Animacion_RetireTarjeta.Location = new Point(0, 405);
                Animacion_RetireTarjeta.SizeMode = PictureBoxSizeMode.StretchImage;



                //   Animacion_Principal.Dock = DockStyle.Fill;
                //Animacion_PublicidadSecundaria.Size = new System.Drawing.Size(1024, 500);
                //Animacion_PublicidadSecundaria.Location = new Point(0, 0);

                //Animacion_RetireTarjeta.Size = new System.Drawing.Size(1024, 380);
                //Animacion_RetireTarjeta.Location = new Point(0, 400);

                Imagen_Fondo.Dock = DockStyle.Fill;
                Imagen_SistemaSuspendido.Dock = DockStyle.Fill;
                Imagen_SistemaSuspendido.BackgroundImageLayout = ImageLayout.Stretch;
                Imagen_DisfruteVisita.Dock = DockStyle.Fill;
                Imagen_DisfruteVisita.BackgroundImageLayout = ImageLayout.Stretch;
                Imagen_VehiculoEntrando.Dock = DockStyle.Fill;
                Imagen_VehiculoEntrando.BackgroundImageLayout = ImageLayout.Stretch;
                Imagen_TarjetaInvalida.Dock = DockStyle.Fill;
                Imagen_TarjetaInvalida.BackgroundImageLayout = ImageLayout.Stretch;
                Imagen_DisfruteVisitaAuto.Dock = DockStyle.Fill;
                Imagen_DisfruteVisitaAuto.BackgroundImageLayout = ImageLayout.Stretch;
                Imagen_TarjetaSinRegistroSalida.Dock = DockStyle.Fill;
                Imagen_TarjetaSinRegistroSalida.BackgroundImageLayout = ImageLayout.Stretch;
                Imagen_AutoVencida.Dock = DockStyle.Fill;
                Imagen_AutoVencida.BackgroundImageLayout = ImageLayout.Stretch;
                Imagen_SinTarjetas.Dock = DockStyle.Fill;
                Imagen_SinTarjetas.BackgroundImageLayout = ImageLayout.Stretch;
                //TbTag.Location = new Point(-7, -34);
                //TbTag.MaxLength = 50;
                //TbTag.Parent = Animacion_Principal;
                tmrTimeOut.Enabled = true;

             
                //tmrEnvioImagenes.Enabled = true;


                ok = true;

                General_Events = "(FrondEnd CargaRecursos): Carga Controles - OK";
            }
            catch (Exception ex)
            {
                General_Events = "Error (FrondEnd CargaRecursos): al cargar recursos. " + ex.ToString();
            }

            return ok;
        }
        private bool CargaImagenes()
        {
            bool ok = false;

            try
            {
                #region Imagenes
                /// Carga Imagenes Flujo
                /// 
                Imagen_Fondo.BackgroundImage = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Jpg\Imagen_Fondo_Principal.jpg"));
                Imagen_SistemaSuspendido.BackgroundImage = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Jpg\Imagen_Sistema_Suspendido.jpg"));
                Imagen_DisfruteVisita.BackgroundImage = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Jpg\Imagen_Disfrute_Visita.jpg"));
                Imagen_VehiculoEntrando.BackgroundImage = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Jpg\Imagen_Vehiculo_Entrando.jpg"));
                Imagen_TarjetaInvalida.BackgroundImage = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Jpg\Imagen_Tarjeta_Invalida.jpg"));
                Imagen_DisfruteVisitaAuto.BackgroundImage = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Jpg\Imagen_Disfrute_VisitaAutorizado.jpg"));
                Imagen_TarjetaSinRegistroSalida.BackgroundImage = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Jpg\Imagen_TarjetaSinRegistroSalida.jpg"));
                Imagen_AutoVencida.BackgroundImage = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Jpg\Imagen_AutorizaicionVencida.jpg"));
                Imagen_SinTarjetas.BackgroundImage = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Jpg\Imagen_SinTarjetas.jpg"));

                #endregion

                #region Botones

                /// Carga botones
                #endregion

                ok = true;

                General_Events = "(FrondEnd CargaImagenes): Carga Imagenes - OK";
            }
            catch (Exception ex)
            {
                General_Events = "Error (FrondEnd CargaImagenes): al cargar imagenes. " + ex.ToString();
            }

            return ok;
        }
        private bool CargaSonidos()
        {
            bool ok = false;

            try
            {
                _sPathBienvenido = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Wav\Sonido_Bienvenido.wav");
                _sPathAutoVencida = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Wav\Sonido_AutoVencida.wav");
                _sPathSinTarjetasDispensador = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Wav\Sonido_SintarjetasDispensador.wav");
                _sPathSistemaSuspendido = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Wav\Sonido_SsistemaSuspendido.wav");
                _sPathTarjetaInvalida = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Wav\Sonido_TarjetaInvalida.wav");
                _sPathTarjetaSinRegistroSalida = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Wav\Sonido_TarjetaSinRegistroSalida.wav");
                _sPathVehiculoEntrando = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Wav\Sonido_VehiculoEntrando.wav");
                _sPathMensualidadUso = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Wav\Sonido_MnesualidadEnUso.wav");
                ok = true;

                //string sMensaje = "(FrondEnd CargaSonidos): Carga sonidos OK";
                //General_Events = sMensaje;
            }
            catch (Exception ex)
            {
                string sMensaje = "Error (FrondEnd CargaSonidos): al cargar sonidos. " + ex.ToString();
                //General_Events = sMensaje;
                //Presentacion = Pantalla.SistemaSuspendido;
                //_ErrorDiagnostico = "Sonidos";
            }
            return ok;
        }
        private bool CargaAnimaciones()
        {
            bool ok = false;

            try
            {


                Animacion_Principal.Image = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Swf\Animacion_Principal.gif"));
                Animacion_PublicidadSecundaria.Image = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Swf\Animacion_PublicidadSecundaria.gif"));
                Animacion_RetireTarjeta.Image = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Swf\Animacion_RetirarTarjeta.gif"));

                //string Principal = (Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Swf\Animacion_Principal.swf"));
                //if (File.Exists(Principal))
                //{
                //    Animacion_Principal.Visible = true;
                //    Animacion_Principal.Movie = Principal;
                //    Animacion_Principal.CtlScale = "ExactFit";
                //    Animacion_Principal.Play();
                //    Animacion_Principal.BringToFront();
                //}

                //string PrincipalSecundario = (Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Swf\Animacion_PublicidadSecundaria.swf"));
                //if (File.Exists(PrincipalSecundario))
                //{
                //    Animacion_PublicidadSecundaria.Visible = true;
                //    Animacion_PublicidadSecundaria.Movie = PrincipalSecundario;
                //    Animacion_PublicidadSecundaria.CtlScale = "ExactFit";
                //    Animacion_PublicidadSecundaria.Play();
                //    Animacion_PublicidadSecundaria.BringToFront();
                //}

                //string RetireTarjeta = (Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Medios\Swf\Animacion_RetirarTarjeta.swf"));
                //if (File.Exists(RetireTarjeta))
                //{
                //    Animacion_RetireTarjeta.Visible = true;
                //    Animacion_RetireTarjeta.Movie = RetireTarjeta;
                //    Animacion_RetireTarjeta.CtlScale = "ExactFit";
                //    Animacion_RetireTarjeta.Play();
                //    Animacion_RetireTarjeta.BringToFront();
                //}


                ok = true;

                //string sMensaje = "(FrondEnd CargaAnimaciones): Carga animaciones OK";
                //General_Events = sMensaje;
            }
            catch (Exception ex)
            {
                //string sMensaje = "Error (FrondEnd CargaAnimaciones): al cargar animaciones. " + ex.ToString();
                //General_Events = sMensaje;
                ok = false;
            }

            return ok;
        }
        private void RestablecerValores()
        {
            _Moto = false;
            bUso = false;
            SecuenciaTransaccion = string.Empty;
            RemoveCard = false;
            lblDatosAuto.Text = string.Empty;
            Ssuspe = 0;
            CntAuto = 0;
            _frmPrincipal_Presenter.LimpiarValoresPLC();
            _sPlaca = string.Empty;
        }
        private async Task<bool> ConectarDispositivos()
        {
            //return true;
            bool ok = false;

            if (Convert.ToBoolean(Globales.sPLC) == true)
            {

                if (_frmPrincipal_Presenter.ConexionPLC())
                {
                    if (_frmPrincipal_Presenter.LimpiarValoresPLC())
                    {
                        _frmPrincipal_Presenter.ConectarDispensadorNew();

                        if (KytReady)
                        {
                            //_frmPrincipal_Presenter.AlistarTarjetaNew();

                            //if (CardKytReady)
                            //{
                            //    _frmPrincipal_Presenter.LimpiarTarjeta();
                            if (_frmPrincipal_Presenter.conexionACR())
                            {
                                ok = true;
                            }
                            //}
                        }
                    }
                }
                if (ok)
                {
                    //if (InicializarLPR())
                    //{
                    //    if (!ConectarLPR())
                    //    {
                    //        General_Events = "(FrontEnd Error ConectarLPR)";
                    //        ok = false;
                    //    }
                    //}
                }
            }
            else
            {
                _frmPrincipal_Presenter.ConectarControl();

                if (_ControlReady)
                {
                    ok = true;
                    //_frmPrincipal_Presenter.ConectarDispensadorNew();

                    //if (KytReady)
                    //{
                    //    //_frmPrincipal_Presenter.AlistarTarjetaNew();

                    //    //if (CardKytReady)
                    //    //{
                    //    //    _frmPrincipal_Presenter.LimpiarTarjeta();
                    //    if (_frmPrincipal_Presenter.conexionACR())
                    //    {
                    //        ok = true;
                    //    }
                    //    //}
                    //}
                }
            }
            return ok;
        }
        private async Task<bool> CapturaCamara(string orden, string Secuencia)
        {
            //return true;
            bool ok = true;

            _frmPrincipal_Presenter.CapturaFoto(orden, Secuencia);
            Camara = false;
            return ok;
        }
        private void RegistroEntrada()
        {
            //Thread ohilo = new Thread(unused => _frmPrincipal_Presenter.CapturaFoto("1", SecuenciaTransaccion));
            //ohilo.Start();

            //SecuenciaTransaccion = DateTime.Now.ToString("yyyyMMddHHmmss") + Globales.sCarril + Globales.iCodigoEstacionamiento;

            oTransaccion.IdAutorizado = 0;
            oTransaccion.CarrilEntrada = Convert.ToInt32(Globales.sCarril);
            oTransaccion.IdEstacionamiento = Convert.ToInt64(Globales.iCodigoEstacionamiento);
            oTransaccion.IdTarjeta = _IdCard;
            oTransaccion.IdTransaccion = Convert.ToInt64(SecuenciaTransaccion);
            oTransaccion.ModuloEntrada = Globales.sSerial;
            oTransaccion.PlacaEntrada = _sPlaca;

            if (VehiculoMoto)
            {
                oTransaccion.TipoVehiculo = 2;
            }
            else
            {
                oTransaccion.TipoVehiculo = 1;
            }
            
            _frmPrincipal_Presenter.RegistrarEntrada(oTransaccion);
            //Imprimir(oTransaccion);
            EliminarPlaca();
            TbTag.Text = "";
            _IdCardAutorizado = "";
            Presentacion = Pantalla.DisfruteVisita;

            if (Convert.ToBoolean(Globales.sPLC) == true)
            {
                _frmPrincipal_Presenter.AbrirTalanquera();
            }
            else
            {
                _frmPrincipal_Presenter.AperturaBarrera();
            }

        }
        private void RegistroEntradaAutorizado(int Conteo)
        {
            /////////////////////////////////////////////////////////////////////////////////////////////////

            oTransaccion.CarrilEntrada = Convert.ToInt32(Globales.sCarril);
            oTransaccion.IdEstacionamiento = Convert.ToInt64(Globales.iCodigoEstacionamiento);
            oTransaccion.IdTarjeta = _lstDtoAutorizado[CntAuto].IdTarjeta;
            oTransaccion.IdAutorizado = _lstDtoAutorizado[CntAuto].IdAutorizacion;
            oTransaccion.IdTransaccion = Convert.ToInt64(SecuenciaTransaccion);
            oTransaccion.ModuloEntrada = Globales.sSerial;
            oTransaccion.PlacaEntrada = _sPlaca;
            if (_Moto)
            {
                oTransaccion.TipoVehiculo = 2;
            }
            else
            {
                oTransaccion.TipoVehiculo = 1;
            }

            _frmPrincipal_Presenter.RegistrarEntrada(oTransaccion);
            EliminarPlaca();
            TbTag.Text = "";
            lblDatosAuto.Text = "Sr/Sra " + _lstDtoAutorizado[CntAuto].NombresAutorizado;
            Presentacion = Pantalla.DisfruteVisitaAuto;


            //Thread ohilo = new Thread(unused => _frmPrincipal_Presenter.CapturaFoto("1", SecuenciaTransaccion));
            //ohilo.Start();  

            if (Convert.ToBoolean(Globales.sPLC) == true)
            {
                _frmPrincipal_Presenter.AbrirTalanquera();
            }
            else
            {
                _frmPrincipal_Presenter.AperturaBarrera();
            }

        }
        //public void CapturaFoto(string Orden)
        //{
        //    string sFileName = string.Empty;

        //    //sFileName = ObtenerValorParametro(Parametros.RutaAlmacenamientoFotos) + sFileName;

        //    sFileName = @"C:\FOTOS\";

        //    if (!Directory.Exists(sFileName))
        //    {
        //        Directory.CreateDirectory(sFileName);
        //    }


        //    sFileName = sFileName + SecuenciaTransaccion + "_" + Globales.sSerial + "_DOMO1_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + Orden + ".jpg";

        //    //string sUrl = "http://" + ObtenerValorParametro(Parametros.IPCamara) + "/cgi-bin/media.cgi?action=getSnapshot";

        //    string sUrl = "http://192.168.1.64/Streaming/channels/101/picture";
        //    if (Generales.WebSiteIsAvailable(sUrl))
        //    {
        //        WebClient webClient = new WebClient();
        //        webClient.Credentials = new NetworkCredential("admin", "D3V3L0P9");
        //        try
        //        {
        //            webClient.DownloadFile(sUrl, sFileName);
        //        }
        //        catch (Exception e)
        //        {
        //            General_Events = "(Presenter CapturaFoto) Exception: " + e.InnerException + " " + e.Message + " " + e.Source;
        //        }
        //    }
        //    else
        //    {
        //        General_Events = "(Presenter CapturaFoto) Error: ALARMA CAMARA IP";
        //    }
        //}
        private void EnvioImagenes()
        {
            string currentDirName = @"C:\FOTOS";

            if (!Directory.Exists(currentDirName))
            {
                Directory.CreateDirectory(currentDirName);
            }

            string[] files = System.IO.Directory.GetFiles(currentDirName, "*.jpg");

            _Transaccion.ModuloEntrada = Globales.sSerial;
            _Transaccion.IdEstacionamiento = Convert.ToInt64(Globales.iCodigoEstacionamiento);

            try
            {
                List<Imagen> olstImagenes = new List<Imagen>();
                List<string> olstImagenesBorrar = new List<string>();

                if (Directory.Exists(currentDirName))
                {
                    DirectoryInfo info = new DirectoryInfo(currentDirName);
                    FileInfo[] oFiles = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
                    if (oFiles.Length > 0)
                    {
                        cnt_timeout = 0;
                        foreach (FileInfo iFile in oFiles)
                        {
                            if (iFile.Name.Contains("Thumbs.db"))
                            {
                                continue;
                            }

                            FileStream fs = new FileStream(iFile.FullName, System.IO.FileMode.Open);
                            Image oImage = Image.FromStream(fs);

                            byte[] bImage = Generales.ImageToByteArray(oImage, ImageFormat.Jpeg);
                            Imagen oImagen = new Imagen();
                            oImagen.ContenidoImagen = bImage;
                            oImagen.NombreImagen = iFile.Name;

                            olstImagenes.Add(oImagen);
                            fs.Close();

                        }

                        if (olstImagenes != null)
                        {
                            if (olstImagenes.Count > 0)
                            {
                                if (_frmPrincipal_Presenter.EnvioImagenes(olstImagenes, _Transaccion))
                                {
                                    foreach (Imagen iImagen in olstImagenes)
                                    {
                                        File.Delete(Path.Combine(currentDirName, iImagen.NombreImagen));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                General_Events = "(ERROR)" + "ERROR AL MOVER ARCHIVO " + ex.ToString();
                Presentacion = Pantalla.SistemaSuspendido;
            }
        }
        private void Sincronizacion()
        {

        }

        private void TabGotFocus(object sender, EventArgs e)
        {
            TbTag.Focus();
        }

             
        #region CamaraEvento
        //private void CapturaPlaca()
        //{


        //    //string Placa = string.Empty;

        //    try
        //    {
        //        #region CapturaPlacaOld
        //        //string ipServer = _frmPrincipal_Presenter.ObtenerValorParametro(Parametros.IPLPR);
        //        //General_Events = "(FrontEnd CapturaPlaca) ipServer: " + ipServer;

        //        //string IDLPR = Globales.sIDLPR;
        //        //Int32 port = Convert.ToInt32(Globales.sPuertoLPR);
        //        //string message = "[lpr;" + IDLPR + ";1;242CF]";
        //        //TcpClient client = new TcpClient(ipServer, port);

        //        //Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
        //        //NetworkStream stream = client.GetStream();
        //        //stream.Write(data, 0, data.Length);
        //        //General_Events = "(FrontEnd CapturaPlaca) message: " + message;

        //        //data = new Byte[256];
        //        //String responseData = String.Empty;

        //        //Int32 bytes = stream.Read(data, 0, data.Length);
        //        //responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        //        //string[] temp = responseData.Split(';');
        //        //Placa = temp[4];
        //        //General_Events = "(FrontEnd CapturaPlaca) Placa: " + Placa;

        //        //if (Placa == string.Empty)
        //        //{
        //        //    Placa = "------";
        //        //}
        //        #endregion

        //        CHCNetSDK.NET_DVR_SETUPALARM_PARAM struAlarmParam = new CHCNetSDK.NET_DVR_SETUPALARM_PARAM();
        //        struAlarmParam.dwSize = (uint)Marshal.SizeOf(struAlarmParam);
        //        struAlarmParam.byLevel = 1; //0- 一级布防,1- 二级布防
        //        struAlarmParam.byAlarmInfoType = 1;//智能交通设备有效，新报警信息类型
        //        struAlarmParam.byFaceAlarmDetection = 1;//1-人脸侦测

        //        //for (int i = 0; i < iDeviceNumber; i++)
        //        //{
        //        m_lUserID = 0;
        //        m_lAlarmHandle[m_lUserID] = CHCNetSDK.NET_DVR_SetupAlarmChan_V41(m_lUserID, ref struAlarmParam);
        //        if (m_lAlarmHandle[m_lUserID] < 0)
        //        {
        //            iLastErr = CHCNetSDK.NET_DVR_GetLastError();
        //            strErr = "Failed to arm, Error code:" + iLastErr; //布防失败，输出错误号
        //        }
        //        else
        //        {
        //            //listViewDevice.Items[i].SubItems[2].Text = "Arm successfully";
        //        }
        //        //    btn_SetAlarm.Enabled = false;
        //        //}


        //    }
        //    catch (Exception ex)
        //    {
        //        General_Events = "(FrontEnd ERROR CapturaPlaca) " + ex.ToString();
        //    }



        //}
        //private void DesarmarLPR()
        //{

        //    m_lUserID = 0;
        //    if (m_lAlarmHandle[m_lUserID] >= 0)
        //    {
        //        if (!CHCNetSDK.NET_DVR_CloseAlarmChan_V30(m_lAlarmHandle[m_lUserID]))
        //        {
        //            iLastErr = CHCNetSDK.NET_DVR_GetLastError();
        //            strErr = "Failed to disarm, Error code:" + iLastErr; //撤防失败，输出错误号
        //            //listViewDevice.Items[i].SubItems[2].Text = strErr;
        //        }
        //        else
        //        {
        //            //listViewDevice.Items[i].SubItems[2].Text = "Disarmed";
        //            m_lAlarmHandle[0] = -1;
        //        }
        //    }
        //    else
        //    {
        //        //listViewDevice.Items[i].SubItems[2].Text = "Disarmed";
        //    }

        //}
        //private bool ConectarLPR()
        //{
        //    bool ok = false;

        //    try
        //    {               //////////////////////////////////////////////////////////////////////////////

        //        string DVRIPAddress = string.Empty;

        //        for (int i = 0; i < _DtoModulo.Partes.Count; i++)
        //        {
        //            if (_DtoModulo.Partes[i].TipoParte == "LPR" && _DtoModulo.Partes[i].Estado)
        //            {
        //                DVRIPAddress = _DtoModulo.Partes[i].IPDispositivo;
        //                break;
        //            }
        //        }

        //        Int16 DVRPortNumber = Int16.Parse(Globales.sPuertoLPR);

        //        string DVRUserName = _frmPrincipal_Presenter.ObtenerValorParametro(Parametros.UsuarioCamaras).ToString();
        //        string DVRPassword = _frmPrincipal_Presenter.ObtenerValorParametro(Parametros.PasswordCamaras).ToString();

        //        CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

        //        m_lUserID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref DeviceInfo);
        //        if (m_lUserID < 0)
        //        {
        //            iLastErr = CHCNetSDK.NET_DVR_GetLastError();
        //            strErr = "NET_DVR_Login_V30 failed, error code= " + iLastErr;

        //        }
        //        else
        //        {
        //            iDeviceNumber++;
        //            string str1 = "" + m_lUserID;
        //            ok = true;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        General_Events = "(FrontEnd ERROR ConectarLPR) " + ex.ToString();
        //    }

        //    return ok;
        //}
        //private bool InicializarLPR()
        //{
        //    bool OK = true;

        //    bool m_bInitSDK = CHCNetSDK.NET_DVR_Init();
        //    if (m_bInitSDK == false)
        //    {

        //        OK = false;
        //    }
        //    else
        //    {
        //        byte[] strIP = new byte[16 * 16];
        //        uint dwValidNum = 0;
        //        Boolean bEnableBind = false;

        //        if (CHCNetSDK.NET_DVR_GetLocalIP(strIP, ref dwValidNum, ref bEnableBind))
        //        {
        //            if (dwValidNum > 0)
        //            {
        //                string ListenIP = "192.168.1.11";
        //                ListenIP = System.Text.Encoding.UTF8.GetString(strIP, 0, 16);
        //                CHCNetSDK.NET_DVR_SetValidIP(0, true);
        //            }

        //        }

        //        CHCNetSDK.NET_DVR_SetLogToFile(3, "C:\\SdkLog\\", true);
        //        for (int i = 0; i < 200; i++)
        //        {
        //            m_lAlarmHandle[i] = -1;
        //        }

        //        if (m_falarmData_V31 == null)
        //        {
        //            m_falarmData_V31 = new CHCNetSDK.MSGCallBack_V31(MsgCallback_V31);
        //        }
        //        CHCNetSDK.NET_DVR_SetDVRMessageCallBack_V31(m_falarmData_V31, IntPtr.Zero);
        //    }

        //    return OK;
        //}
        //public bool MsgCallback_V31(int lCommand, ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        //{
        //    IniciarAlarmaLPR(lCommand, ref pAlarmer, pAlarmInfo, dwBufLen, pUser);

        //    return true;
        //}
        ////Inicializar alarma para obtener la placa 
        //public void IniciarAlarmaLPR(int lCommand, ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        //{
        //    switch (lCommand)
        //    {
        //        case CHCNetSDK.COMM_ITS_PLATE_RESULT:
        //            ProcessCommAlarm_ITSPlate(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
        //            break;

        //        default:
        //            {
        //                string strIP = pAlarmer.sDeviceIP;

        //                string stringAlarm = "upload alarm，alarm message type：" + lCommand;

        //                if (InvokeRequired)
        //                {
        //                    object[] paras = new object[3];
        //                    paras[0] = DateTime.Now.ToString();
        //                    paras[1] = strIP;
        //                    paras[2] = stringAlarm;
        //                }
        //                else
        //                {

        //                    UpdateClientList(DateTime.Now.ToString(), strIP, stringAlarm);
        //                }
        //            }
        //            break;
        //    }
        //}
        ////Proceso para obtener la placa 
        //private void ProcessCommAlarm_ITSPlate(ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        //{
        //    CHCNetSDK.NET_ITS_PLATE_RESULT struITSPlateResult = new CHCNetSDK.NET_ITS_PLATE_RESULT();
        //    uint dwSize = (uint)Marshal.SizeOf(struITSPlateResult);
        //    struITSPlateResult = (CHCNetSDK.NET_ITS_PLATE_RESULT)Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_ITS_PLATE_RESULT));
        //    string strIP = pAlarmer.sDeviceIP;
        //    string stringPlateLicense = System.Text.Encoding.GetEncoding("GBK").GetString(struITSPlateResult.struPlateInfo.sLicense).TrimEnd('\0');
        //    _sPlaca = stringPlateLicense;
        //    General_Events = "(FrontEnd CapturaPlaca) Placa: " + _sPlaca;

        //}
        //public void UpdateClientList(string strAlarmTime, string strDevIP, string strAlarmMsg)
        //{
        //    //listViewAlarmInfo.Items.Add(new ListViewItem(new string[] { strAlarmTime, strDevIP, strAlarmMsg }));
        //}

        #endregion
        #endregion

        #region IView
        public Pantalla Presentacion
        {
            set
            {
                SoundPlayer simpleSound;

                switch (value)
                {

                    case Pantalla.SalvaPantallas:
                        _cnt_timeout = 0;                                           
                        Animacion_Principal.BringToFront();
                        RestablecerValores();
                        break;

                    case Pantalla.RetireTarjeta:
                        _cnt_timeout = 0;
                        Imagen_Fondo.BringToFront();
                        Animacion_PublicidadSecundaria.BringToFront();
                        Animacion_RetireTarjeta.BringToFront();
                        break;

                    case Pantalla.SistemaSuspendido:
                        _cnt_timeout = 0;

                        if (!soundSuspen)
                        {
                            simpleSound = new SoundPlayer(_sPathSistemaSuspendido);
                            simpleSound.Play();
                            soundSuspen = true;
                        }

                        Imagen_SistemaSuspendido.BringToFront();
                        break;

                    case Pantalla.DisfruteVisita:
                        _cnt_timeout = 0;
                        Imagen_DisfruteVisita.BringToFront();
                        break;

                    case Pantalla.VehiculoEntrando:
                        _cnt_timeout = 0;
                        Imagen_VehiculoEntrando.BringToFront();
                        break;

                    case Pantalla.TarjetaInvalida:
                        _cnt_timeout = 0;
                        Imagen_TarjetaInvalida.BringToFront();
                        break;

                    case Pantalla.DisfruteVisitaAuto:
                        _cnt_timeout = 0;
                        Imagen_DisfruteVisitaAuto.BringToFront();
                        lblDatosAuto.Parent = Imagen_DisfruteVisitaAuto;
                        lblBienvenido.Parent = Imagen_DisfruteVisitaAuto;
                        lblDatosAuto.BringToFront();
                        lblBienvenido.BringToFront();
                        break;

                    case Pantalla.TarjetaSinRegistroSalida:
                        _cnt_timeout = 0;
                        Imagen_TarjetaSinRegistroSalida.BringToFront();
                        break;
                    case Pantalla.AutorizacionVencida:
                        _cnt_timeout = 0;
                        Imagen_AutoVencida.BringToFront();
                        break;
                    case Pantalla.SinTarjetas:
                        _cnt_timeout = 0;

                        if (!soundSinTar)
                        {
                            simpleSound = new SoundPlayer(_sPathSinTarjetasDispensador);
                            simpleSound.Play();
                            soundSinTar = true;
                        }

                        Imagen_SinTarjetas.BringToFront();
                        break;
                }

                _Presentacion = value;
            }
            get
            {
                return _Presentacion;
            }
        }
        public string General_Events
        {
            set
            {
                TraceHandler.WriteLine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Logs\Log"), "MENSAJE: " + value, TipoLog.TRAZA);
            }
        }

        public void Imprimir(Transaccion transaccion)
        {
            try
            {
                CapturaRutaBarras();
                General_Events = "FrondEnd-Imprimir -> Funcion imprimir Ticket.";
                ReportDataSource dataSource = new ReportDataSource();                
                LocalReport localReport = new LocalReport();
                dataSource = new ReportDataSource("DataSetEntrada", _frmPrincipal_Presenter.GenerarTicketEntrada(transaccion).Tables[0]);
                localReport.ReportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format(@"Tickets\{0}.rdlc", "TicketEntrada"));
                ReportParameter urlImage = new ReportParameter("imgUrl", new Uri(Convert.ToString(_imgUrl)).AbsoluteUri);
                localReport.EnableExternalImages = true;
                localReport.SetParameters(new ReportParameter[] { urlImage });
                localReport.Refresh();
                localReport.DataSources.Add(dataSource);
            
                ReportPrintDocument ore = new ReportPrintDocument(localReport);
                //ore.PrinterSettings.PrinterName = Globales.sNombreImpresoraTickets;
                ore.PrintController = new StandardPrintController();
                ore.Print();
                ore.Dispose();
                ore = null;
                localReport.Dispose();
                EliminarCodigoBarras();

            }
            catch (Exception ex )
            {

                General_Events = "Error- FrondEnd-Imprimir ->" + ex.ToString();
            }
        }

        public void CapturaPlacaNew()
        {

            #region Old
            //try
            //{
            //    string IpCamera = string.Empty;

            //    for (int i = 0; i < _DtoModulo.Partes.Count; i++)
            //    {
            //        if (_DtoModulo.Partes[i].TipoParte == "LPR" && _DtoModulo.Partes[i].Estado)
            //        {
            //            IpCamera = _DtoModulo.Partes[i].IPDispositivo;
            //            break;
            //        }
            //    }
            //    string UserName = _frmPrincipal_Presenter.ObtenerValorParametro(Parametros.UsuarioCamaras).ToString();
            //    string Password = _frmPrincipal_Presenter.ObtenerValorParametro(Parametros.PasswordCamaras).ToString();

            //    string strUrl = "http://" + IpCamera + "/ISAPI/Traffic/channels/1/vehicleDetect/plates";
            //    string param = "<AfterTime></AfterTime>";
            //    WebClient client = new WebClient();
            //    // Set the user name and password
            //    client.Credentials = new NetworkCredential("" + UserName + "", "" + Password + "");
            //    string responseData = client.UploadString(strUrl, param);

            //    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Plates));
            //    using (StringReader textReader = new StringReader(responseData))
            //    {
            //        var rta = (Plates)xmlSerializer.Deserialize(textReader);
            //        _sPlaca = rta.Plate[rta.Plate.Count - 1].PlateNumber;
            //        General_Events = "(FrontEnd CapturaPlaca) Placa: " + _sPlaca;

            //    }

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            #endregion           
            string Placa = string.Empty;
            string rutaPlaca = string.Empty;


            for (int i = 0; i < _DtoModulo.Parametros.Count; i++)
            {
                if (_DtoModulo.Parametros[i].Codigo == "Placas" + Globales.sSerial + "" && _DtoModulo.Parametros[i].Estado)
                {
                    rutaPlaca = _DtoModulo.Parametros[i].Valor;
                    break;
                }
            }
            //LEER OLD
            //TextReader leer = new StreamReader("" + rutaPlaca + "" + "" + Globales.sSerial + "" + ".txt");
            //Placa = leer.ReadToEnd();
            //_sPlaca = Placa.TrimEnd();

            if (File.Exists(rutaPlaca + Globales.sSerial + ".txt"))
            {
                using (StreamReader leer = new StreamReader(rutaPlaca + Globales.sSerial + ".txt"))
                {
                    string placa = leer.ReadToEnd();
                    _sPlaca = placa.TrimEnd();
                }
            }
            else
            {
                _sPlaca = "------";
            }

        }
        public void EliminarPlaca()
        {
            #region Old
            //try
            //{
            //    string IpCamera = string.Empty;

            //    for (int i = 0; i < _DtoModulo.Partes.Count; i++)
            //    {
            //        if (_DtoModulo.Partes[i].TipoParte == "LPR" && _DtoModulo.Partes[i].Estado)
            //        {
            //            IpCamera = _DtoModulo.Partes[i].IPDispositivo;
            //            break;
            //        }
            //    }
            //    string UserName = _frmPrincipal_Presenter.ObtenerValorParametro(Parametros.UsuarioCamaras).ToString();
            //    string Password = _frmPrincipal_Presenter.ObtenerValorParametro(Parametros.PasswordCamaras).ToString();

            //    string strUrl = "http://" + IpCamera + "/ISAPI/Traffic/channels/1/vehicleDetect/plates";
            //    string param = "<AfterTime></AfterTime>";
            //    WebClient client = new WebClient();
            //    // Set the user name and password
            //    client.Credentials = new NetworkCredential("" + UserName + "", "" + Password + "");
            //    string responseData = client.UploadString(strUrl, param);

            //    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Plates));
            //    using (StringReader textReader = new StringReader(responseData))
            //    {
            //        var rta = (Plates)xmlSerializer.Deserialize(textReader);
            //        _sPlaca = rta.Plate[rta.Plate.Count - 1].PlateNumber;
            //        General_Events = "(FrontEnd CapturaPlaca) Placa: " + _sPlaca;

            //    }

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            #endregion

            string Placa = string.Empty;
            string rutaPlaca = string.Empty;


            for (int i = 0; i < _DtoModulo.Parametros.Count; i++)
            {
                if (_DtoModulo.Parametros[i].Codigo == "Placas" + Globales.sSerial + "" && _DtoModulo.Parametros[i].Estado)
                {
                    rutaPlaca = _DtoModulo.Parametros[i].Valor;
                    break;
                }
            }
            //LEER OLD
            //TextReader leer = new StreamReader("" + rutaPlaca + "" + "" + Globales.sSerial + "" + ".txt");
            //Placa = leer.ReadToEnd();
            //_sPlaca = Placa.TrimEnd();
            string rutaPlacaGuardada = rutaPlaca + Globales.sSerial + ".txt";
            if (File.Exists(rutaPlacaGuardada))
            {
                File.Delete(rutaPlacaGuardada);
            }

        }

        public void CapturaRutaBarras()
        {

            #region Old
            //try
            //{
            //    string IpCamera = string.Empty;

            //    for (int i = 0; i < _DtoModulo.Partes.Count; i++)
            //    {
            //        if (_DtoModulo.Partes[i].TipoParte == "LPR" && _DtoModulo.Partes[i].Estado)
            //        {
            //            IpCamera = _DtoModulo.Partes[i].IPDispositivo;
            //            break;
            //        }
            //    }
            //    string UserName = _frmPrincipal_Presenter.ObtenerValorParametro(Parametros.UsuarioCamaras).ToString();
            //    string Password = _frmPrincipal_Presenter.ObtenerValorParametro(Parametros.PasswordCamaras).ToString();

            //    string strUrl = "http://" + IpCamera + "/ISAPI/Traffic/channels/1/vehicleDetect/plates";
            //    string param = "<AfterTime></AfterTime>";
            //    WebClient client = new WebClient();
            //    // Set the user name and password
            //    client.Credentials = new NetworkCredential("" + UserName + "", "" + Password + "");
            //    string responseData = client.UploadString(strUrl, param);

            //    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Plates));
            //    using (StringReader textReader = new StringReader(responseData))
            //    {
            //        var rta = (Plates)xmlSerializer.Deserialize(textReader);
            //        _sPlaca = rta.Plate[rta.Plate.Count - 1].PlateNumber;
            //        General_Events = "(FrontEnd CapturaPlaca) Placa: " + _sPlaca;

            //    }

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            #endregion
            string Placa = string.Empty;
            string rutaBarras = string.Empty;


            for (int i = 0; i < _DtoModulo.Parametros.Count; i++)
            {
                if (_DtoModulo.Parametros[i].Codigo == "CodigoBarras" + Globales.sSerial + "" && _DtoModulo.Parametros[i].Estado)
                {
                    rutaBarras = _DtoModulo.Parametros[i].Valor;
                    break;
                }
            }
            _imgUrl = rutaBarras;

        }
        public void EliminarCodigoBarras()
        {  //LEER OLD
            //TextReader leer = new StreamReader("" + rutaPlaca + "" + "" + Globales.sSerial + "" + ".txt");
            //Placa = leer.ReadToEnd();
            //_sPlaca = Placa.TrimEnd();
            string rutaPlacaGuardada = imgUrl;
            if (File.Exists(rutaPlacaGuardada))
            {
                File.Delete(rutaPlacaGuardada);
            }

        }
        #endregion

        #region PLC
        private void btn_llegoCarro_Click(object sender, EventArgs e)
        {
            SecuenciaTransaccion = DateTime.Now.ToString("yyyyMMddHHmmss") + oTransaccion.CarrilEntrada + 2;

            Presentacion = Pantalla.RetireTarjeta;


            //_Tarjeta.CarrilEntrada = Globales.sCarril;
            //_Tarjeta.CicloActivo = true;
            //_Tarjeta.FechaHoraEntrada = DateTime.Now;
            //_Tarjeta.ModuloEntrada = Globales.sSerial;


            _frmPrincipal_Presenter.EscribirCard(_Tarjeta, false);

            if (_WriteCard)
            {
                _frmPrincipal_Presenter.DispensarTarjeta();
            }
            else
            {
                _frmPrincipal_Presenter.AlistarTarjeta();
            }

            //CapturaFoto("1");

        }
        private void btn_CarroBarrera_Click(object sender, EventArgs e)
        {
            Presentacion = Pantalla.VehiculoEntrando;
            _frmPrincipal_Presenter.AlistarTarjeta();
            _frmPrincipal_Presenter.LimpiarTarjeta();
            //CapturaFoto("2");
        }
        private void btn_CarroFuera_Click(object sender, EventArgs e)
        {
            //CapturaFoto("3");
            Presentacion = Pantalla.SalvaPantallas;
        }
        #endregion
    }
}

