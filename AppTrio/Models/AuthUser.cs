using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTrio.Models
{
	[Serializable]
	public class AuthUser
	{
		public string Login { get; set; }
		public string Email { get; set; }
		public string Category { get; set; }
		public string Post { get; set; }
		public string FIO { get; set; }
		public string Mobile { get; set; }

		public AuthUser(){}

		public AuthUser(string login, string email, string category, string post, string fIO, string mobile)
		{
			Login = login;
			Email = email;
			Category = category;
			Post = post;
			FIO = fIO;
			Mobile = mobile;
		}
	}
}
