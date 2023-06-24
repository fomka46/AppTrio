using AppTrio.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace AppTrio
{
	/// <summary>
	/// Логика взаимодействия для AuthWindow.xaml
	/// </summary>
	public partial class AuthWindow : Window
	{
		public AuthWindow()
		{
			InitializeComponent();
			if (File.Exists("user.xml"))
				ShowMainWindow();
		}

		private void Auth_Btn_Click(object sender, RoutedEventArgs e)
		{
			string login = Auth_LoginField.Text.Trim();
			string password = Auth_PassField.Password.Trim();

			if (login.Equals(""))
			{
				ShakeEffect<TextBox>(Auth_LoginField);
				return;
			}
			else if (password.Length < 3)
			{
				ShakeEffect<PasswordBox>(Auth_PassField);
				return;
			}
			User authuser = null;
			using (AppDbContext db = new AppDbContext())
			{
				authuser = db.Users.Where(user => user.Login == login && user.Password == Hash(password)).FirstOrDefault();
			}
			if (authuser == null)
			{
				MessageBox.Show("Такого пользователя не существет");
			}
			else
			{
				AuthUser auth = new AuthUser(login, authuser.Email,authuser.Category,authuser.Post,authuser.FIO,authuser.Mobile);
				XmlSerializer xml = new XmlSerializer(typeof(AuthUser));
				using (FileStream file = new FileStream("user.xml", FileMode.CreateNew))
				{
					xml.Serialize(file, auth);
				}
				ShowMainWindow();
			}

		}
		private void ShowMainWindow()
		{
			Hide();
			MainWindow window = new MainWindow();
			window.Show();
			Close();
		}
		private string Hash(string input)//хеширование пароля скрытие пароля
		{
			byte[] temp = Encoding.UTF8.GetBytes(input);
			using (SHA1Managed sha1 = new SHA1Managed())
			{
				var hash = sha1.ComputeHash(temp);
				return Convert.ToBase64String(hash);
			}
		}
		private void ShakeEffect<Type>(Type widget)// тряска поля с ошибкой
		{
			DoubleAnimation arim = new DoubleAnimation();
			arim.From = 0;
			arim.To = 5;
			arim.Duration = TimeSpan.FromMilliseconds(200);
			arim.RepeatBehavior = new RepeatBehavior(3);
			arim.AutoReverse = true;
			var rotate = new RotateTransform();
			if (widget is TextBox)
				(widget as TextBox).RenderTransform = rotate;
			else if (widget is PasswordBox)
				(widget as PasswordBox).RenderTransform = rotate;
			else
				throw new Exception("Type is not valid");

			rotate.BeginAnimation(RotateTransform.AngleProperty, arim);
		}


	}
}
