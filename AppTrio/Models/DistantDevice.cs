using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTrio.Models
{
	internal class DistantDevice
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string WayConnect { get; set; }
		public string Login { get; set; }
		
		public string Password { get; set; }
		
		public string Coment { get; set; }

		public DistantDevice()
		{
		}

		public DistantDevice(string name, string wayConnect, string login, string password, string coment)
		{
			Name = name;
			WayConnect = wayConnect;
			Login = login;
			Password = password;
			Coment = coment;
		}
	}

}
