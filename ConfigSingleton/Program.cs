using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConfigSingleton
{
    class Program
    {
        
        static void Main(string[] args)
        {
            String leido = String.Empty;
            do
            {
                PrintMensaje();
                leido = Console.ReadLine();

                switch (leido)
                {
                    case "1":
                        //Añadir Nueva config:
                        NewConfig();
                        break;
                    case "2":
                        //Listar Configs
                        ListarConfigs();
                        break;
                    case "3":
                        //Hacemos persistente la lista de configs. 
                        GuardarConfigs();
                        leido = "4";
                        break;
                }
            } while (leido != "4");
        }

        static void PrintMensaje()
        {
            Console.WriteLine("************** El patrón Singleton (ejemplo práctico) *****************");
            Console.WriteLine("*                                                                     *");
            Console.WriteLine("*    En este ejemplo usaremos el patrón Singleton para un Gestor de   *");
            Console.WriteLine("*       configuraciones que podría ser el global a una aplicación.    *");
            Console.WriteLine("*                                                                     *");
            Console.WriteLine("*                        Selecciona una opción:                       *");
            Console.WriteLine("*                        1) Añadir una config                         *");
            Console.WriteLine("*                        2) Listar las configs                        *");
            Console.WriteLine("*                        3) Guardar y salir                           *");
            Console.WriteLine("*                        4) Salir sin guardar                         *");
            Console.WriteLine("***********************************************************************");
        }

        static void NewConfig()
        {
            bool _correcto = false;
            String idConfig = String.Empty;
            String configValue = String.Empty;
            do
            {
                Console.WriteLine("AVISO -> Si la config introducida ya existe se duplicará la misma.");
                Console.WriteLine("Introduzca el identificador de la nueva config:");
                idConfig = Console.ReadLine();
                Console.WriteLine("Introduzca el valor de la nueva config:");
                configValue = Console.ReadLine();

                //Establecemos la corrección de los datos. 
                _correcto = String.IsNullOrEmpty(idConfig) && String.IsNullOrEmpty(configValue);

                //Intentamos añadir la config. 
                GestorConfig.Instancia.AddConfig(new Config(idConfig, configValue));
            } while (_correcto);
        }

        static void ListarConfigs()
        {
            GestorConfig.Instancia.GetConfigs().ForEach(
                conf => Console.WriteLine("Config: {0} <=> Valor: {1}", conf.Id, conf.Value));
        }

        static void GuardarConfigs()
        {
            GestorConfig.Instancia.PersistConfigs();
        }
    }

    public class GestorConfig
    {
        private static string _filexml = "config.xml";
        //Los ejemplos indican que hay que igualarla a null. 
        private static GestorConfig _instancia;
        private static List<Config> _configs; 
		
		static GestorConfig()
        {
            //Inicializamos el listado de las configs y cargamos las que tuviéramos en el fichero config.xml si existe. 
            _configs = new List<Config>();
            LoadConfigs();
        }
		
		//Propiedad accesible para recuperar la instancia de la clase o crearla si no existe. 
        public static GestorConfig Instancia
        {
            get
            {
                if (_instancia == null)
                    _instancia = new GestorConfig();

                return _instancia;
            }
        }
		
		private static void LoadConfigs(){
            //Carga Inicial de las configs desde el fichero. 
            XDocument xdoc = XDocument.Load(_filexml);
            xdoc.Element("Root").Descendants().ToList().ForEach(elem => Instancia.AddConfig(new Config( elem.Name.ToString(),elem.Value)));
		}

        public void PersistConfigs(){
            XDocument xdoc;
            //Comprobamos si existe el archivo
            if (!System.IO.File.Exists(_filexml))
            {
                xdoc = new XDocument(new XElement("Root"));
                xdoc.Save(_filexml);
            }
            xdoc = XDocument.Load(_filexml);
            //Hacemos que las configuraciones establecidas se conviertan en persistentes. 
            xdoc = XDocument.Load(_filexml);
            XElement root = xdoc.Element("Root");
            XElement elem = null;
            foreach (Config conf in _configs)
            {
                elem = new XElement(conf.Id);
                elem.Value = conf.Value;
                root.Add(elem);
            }
            xdoc.Save(_filexml);
        }

        public void AddConfig(Config conf)
        {
            //Añadimos una config a la lista (en ejecución)
            _configs.Add(conf);
        }

        public List<Config> GetConfigs()
        {
            //Recuperamos la lista de configs (en ejecucíón)
            return _configs;
        }
    }

    public class Config
    {
        public Config(String _id, String _value)
        {
            Id = _id;
            Value = _value;
        }

        public String Id { get; set; }

        public String Value { get; set; }
    }

    
}
