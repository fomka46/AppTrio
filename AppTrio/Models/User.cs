using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTrio.Models
{
	internal class User
	{
		public int Id { get; set; }
		public string Login { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Category { get; set; }
		public string Post {get; set; }
		public string FIO { get; set; }
		public string Mobile {get; set; }

		public User() { }

		public User(string login, string email, string password, string category,string post, string fio,string mobile)
		{
			Login = login;
			Email = email;
			Password = password;
			Category = category;
			Post = post;
			FIO = fio;
			Mobile = mobile;
		}
	}
}
