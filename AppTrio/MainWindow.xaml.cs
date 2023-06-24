using AppTrio.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace AppTrio
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	
	public partial class MainWindow : Window
	{
		private AppDbContext _db;
		//----------------------------------------------------------------------------------
		private int IdDistanDivace = -1;//для сравневания выбранного элемента в listbox удаленных устройств элемента в словаре 
		private Dictionary<int, string> Title_DistanDivace = new Dictionary<int, string>(); // словарь для выборки полей и listbox удаленных устройств
		//----------------------------------------------------------------------------------
		public MainWindow()
		{
			InitializeComponent();
			MainScreen.IsChecked = true;
			_db = new AppDbContext();
			if (!File.Exists("user.xml"))
			{
				ShowAuthWindow();
			}
			XmlSerializer xml = new XmlSerializer(typeof(AuthUser));
			using (FileStream file = new FileStream("user.xml", FileMode.Open))
			{
				AuthUser auth = (AuthUser)xml.Deserialize(file);
				AuthUserNameLabel.Content = auth.Login;
			}
		}

		private void RadioBtn_Checked(object sender, RoutedEventArgs e) // проверка какая кнопка нажата и открыте нужной панели
		{
			string objName = ((RadioButton)sender).Name;
			StackPanel[] panels = { UserScreen_Window, DistantScreen_Window };
			foreach (var panel in panels)
			{
				panel.Visibility = Visibility.Hidden;
			}
			switch (objName)
			{
				case "UserScreen":
					{
						VisibleScrol("ScrolUserScreen_Window");
						UserScreen_Window.Visibility = Visibility.Visible;
						ActiveWindowLabel.Content = "Личный кабинет";
						DeserializerUserCabinet();
						break;
					}
				case "DistantScreen":
					{
						VisibleScrol("ScrolDistantScreen_Window");
						DistantScreen_Window.Visibility = Visibility.Visible;
						ViewListDistanDevice();
						ActiveWindowLabel.Content = "Параметры подключения к удаленным устройствам";
						break;
					}

			}//DistantScreen_Window
		}
		private void VisibleScrol(string namescrol) // отображение ScrollViewer каждой панели
		{
			if(namescrol == "ScrolUserScreen_Window")
			{
				ScrolDistantScreen_Window.Visibility = Visibility.Hidden;
				ScrolUserScreen_Window.Visibility = Visibility.Visible;
			}
			else if (namescrol == "ScrolDistantScreen_Window")
			{
				ScrolUserScreen_Window.Visibility= Visibility.Hidden;
				ScrolDistantScreen_Window.Visibility=Visibility.Visible;
			}
		}
		private string Hash(string input) // Хеширование пароля
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
		private void ReceiveUserLogin()// получение списка логинов пользователей
		{
			List<string> userlogin = new List<string>();
			List <User> users = _db.Users.ToList();
			foreach (var user in users)
			{
				userlogin.Add(user.Login);
				
			}
			UserScreenDel_LoginField_ComboBox.ItemsSource = userlogin;
		}
		private void ShowAuthWindow()
		{
			Hide();
			AuthWindow window = new AuthWindow();
			window.Show();
			Close();
		}// открытие окна авторизации

		 // ___________________ Работы с окном личного кабинета ___________________
		private void UserScreen_RegisterUser_Click(object sender, RoutedEventArgs e)
		{
			UserScreenDel_LoginField_ComboBox.Text = "";
			UserScreen_DelPanel.Visibility = Visibility.Collapsed;
			UserScreen_RegisterPanel.Visibility = Visibility.Visible;
			
		}// Переход на вкладку регистрации пользователя в личном кабинете для администратора

		private void UserScreen_DeleteUser_Click(object sender, RoutedEventArgs e)
		{
			ReceiveUserLogin();
			UserScreen_RegisterPanel.Visibility = Visibility.Collapsed;
			UserScreen_DelPanel.Visibility = Visibility.Visible;


		}//Переход на вкладку удаления пользователя в личном кабинете для администратора
		private void Register_RegisterBtn_Click(object sender, RoutedEventArgs e) // кнопка регистрации пользователя
		{

			string login = Register_LoginField.Text.Trim();
			string email = Register_EmailField.Text.Trim();
			string pass = Register_PassField.Password.Trim();
			string category = Register_CategoryField.Text.Trim();
			string post = Register_PostField.Text.Trim();
			string fio = Register_FioField.Text.Trim();
			string mobile = Register_MobileField.Text.Trim();
			
			
			if(login.Equals(""))
			{
				ShakeEffect<TextBox>(Register_LoginField);
				return;
			}
			else if(pass.Length < 3 || pass.Equals(""))
			{
				ShakeEffect<PasswordBox>(Register_PassField);
				return;
			}
			else if (category.Equals(""))
			{
				MessageBox.Show("Забыли выбрать должностью");
				return;
			}
			else if (post.Equals(""))
			{
				ShakeEffect<TextBox>(Register_PostField);
				return;
			}
			else if (fio.Equals(""))
			{
				ShakeEffect<TextBox>(Register_FioField);
				return;
			}
			else if (mobile.Equals(""))
			{
				ShakeEffect<TextBox>(Register_MobileField);
				return;
			}
			User registerUser = _db.Users.Where(el => el.Login == login).FirstOrDefault();
			if (registerUser != null)
			{
				MessageBox.Show("Пользователь с таким логином уже существует");
				return;
			}
			try
			{
				User newuser = new User(login, email, Hash(pass), category, post, fio, mobile);
				_db.Users.Add(newuser);
				_db.SaveChanges();
				MessageBox.Show("Пользователь зарегистрирован успешно");
			}
			catch (Exception ex) { MessageBox.Show(ex.Message); return; }
			Register_LoginField.Text="";
			Register_EmailField.Text = "";
			Register_PassField.Password = "";
			Register_CategoryField.Text = "";
			Register_PostField.Text = "";
			Register_FioField.Text = "";
			Register_MobileField.Text = "";
			UserScreen_RegisterPanel.Visibility= Visibility.Collapsed;
		}

		private void UserScreenDel_DelBtn_Click(object sender, RoutedEventArgs e) // кнопка удаления пользователя
		{
			string deluserlogin = UserScreenDel_LoginField_ComboBox.Text.Trim();
			if (deluserlogin.Equals(""))
			{
				MessageBox.Show("Вы не выбрали пользователя для удаления");
				return;
			}
			try
			{
				var userdel = _db.Users.SingleOrDefault(x => x.Login == deluserlogin);
				if (userdel != null)
				{
					_db.Users.Remove(userdel);
					_db.SaveChanges();
					MessageBox.Show($"Пользователь с логином {deluserlogin} успешно удален");
					UserScreenDel_LoginField_ComboBox.Text = "";
					ReceiveUserLogin();
				}
			}catch (Exception ex){MessageBox.Show(ex.Message); return;}
			UserScreen_DelPanel.Visibility = Visibility.Collapsed;
		}

		private void UserScreen_ReLogin_Click(object sender, RoutedEventArgs e) // кнопка выхода из аккаунта
		{
			File.Delete("user.xml");
			ShowAuthWindow();

		}
		private void DeserializerUserCabinet()//десериализация файла подключения пользователя
		{
			XmlSerializer xml = new XmlSerializer(typeof(AuthUser));
			using (FileStream file = new FileStream("user.xml", FileMode.Open))
			{
				AuthUser auth = (AuthUser)xml.Deserialize(file);
				UserScreen_Fio.Content = auth.FIO;
				UserScreen_Post.Content = auth.Post;
				UserScreen_Category.Content = auth.Category;
				UserScreen_Mobile.Content = auth.Mobile;
				if(auth.Category == "Администратор")
				{
					UserScreen_AdminPanel.Visibility= Visibility.Visible;
				}
			}
		}

   // ___________________ Работы с окном удаленный доступ ___________________
		private void AddNewDistant_Btn_Click(object sender, RoutedEventArgs e)// отображение панели добавление удаленного устройства
		{
			AddNewDistant_Panel.Visibility = Visibility.Visible;
		}
		private void NewDistant_SaveBtn_Click(object sender, RoutedEventArgs e)// добавление устройства в бд
		{
			string name = NewDistant_NameField.Text.Trim();
			string WayConnect = NewDistant_WayConnectField.Text.Trim();
			string login = NewDistant_LoginField.Text.Trim();
			string password = NewDistant_PassField.Text.Trim();
			string coment = NewDistant_ComentField.Text.Trim();

			if (name.Equals(""))
			{
				ShakeEffect<TextBox>(NewDistant_NameField);
				return;
			}
			else if (WayConnect.Equals(""))
			{
				MessageBox.Show("Выберите способ подключения");
				return;
			}
			else if (login.Equals(""))
			{
				ShakeEffect<TextBox> (NewDistant_LoginField);
				return;
			}
			else if (password.Equals(""))
			{
				ShakeEffect<TextBox>(NewDistant_PassField);
				return;
			}
			try
			{
				DistantDevice newDistantDevice = new DistantDevice(name, WayConnect, login, password, coment);
				_db.DistantDevice.Add(newDistantDevice);
				_db.SaveChanges();
				MessageBox.Show("Устройство успешно добавлено");
			}catch(Exception ex) { MessageBox.Show(ex.Message); return;}
			NewDistant_NameField.Text = "";
			NewDistant_WayConnectField.Text = "";
			NewDistant_LoginField.Text = "";
			NewDistant_PassField.Text = "";
			NewDistant_ComentField.Text = "";
			AddNewDistant_Panel.Visibility = Visibility.Collapsed;
			DistantDivaceList.ItemsSource = "";
			ViewListDistanDevice();

		}
		public void ViewListDistanDevice()// вывод списка удаленных устройств
		{
			List<DistantDevice> divace_items = _db.DistantDevice.ToList(); // Получение списка товаров

			//создаем объект на основе ObservableCollection
			ObservableCollection<DistantDevice> listdistandivace = new ObservableCollection<DistantDevice>();

			// Перебираем весь список и добавляем каждый элемент в ObservableCollection
			foreach (DistantDevice item in divace_items)
			{
				IdDistanDivace++;
				Title_DistanDivace.Add(IdDistanDivace, item.Name);
				listdistandivace.Add(item);

			}
			DistantDivaceList.ItemsSource = listdistandivace;
		}
		
	}

}
