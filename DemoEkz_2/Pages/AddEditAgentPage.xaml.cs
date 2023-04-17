using DemoEkz_2.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DemoEkz_2.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditAgentPage.xaml
    /// </summary>
    public partial class AddEditAgentPage : Page
    {
        public readonly DbEntities _db = DbEntities.GetContext();
        public readonly Agent _agent;
        public AddEditAgentPage(Agent agent)
        {
            InitializeComponent();
            _agent = agent;
            txtTitle.Text = _agent.Title;
            _db.AgentTypes.Load();
            cmbAgentType.ItemsSource = _db.AgentTypes.Local.ToList();
            cmbAgentType.SelectedItem= _agent.AgentType;
            txtAdress.Text = _agent.Address;
            txtINN.Text = _agent.INN;
            txtKPP.Text = _agent.KPP;
            txtDirectorName.Text = _agent.DirectorName;
            txtPhone.Text = _agent.Phone;
            txtEmail.Text = _agent.Email;
            cmbLogo.ItemsSource = _db.Agents.Local.ToList();
            cmbLogo.SelectedItem = _agent;
            txtPriority.Text = _agent.Priority.ToString();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrEmpty(txtTitle.Text))
            {
                errors.AppendLine("Введите наименование");
            }
            if (cmbAgentType.SelectedItem == null)
            {
                errors.AppendLine("Выберите тип агента");
            }
            if (string.IsNullOrEmpty(txtAdress.Text))
            {
                errors.AppendLine("Введите адресс");
            }
            if (string.IsNullOrEmpty(txtINN.Text))
            {
                errors.AppendLine("Введите ИНН");
            }
            if (string.IsNullOrEmpty(txtKPP.Text))
            {
                errors.AppendLine("Введите КПП");
            }
            if (string.IsNullOrEmpty(txtDirectorName.Text))
            {
                errors.AppendLine("Введите имя директора");
            }
            if (string.IsNullOrEmpty(txtPhone.Text))
            {
                errors.AppendLine("Введите телефон");
            }
            if (string.IsNullOrEmpty(txtEmail.Text))
            {
                errors.AppendLine("Введите почту");
            }
            if (!int.TryParse(txtPriority.Text, out int priority) || priority < 0)
            {
                errors.AppendLine("Приоритет введен неверно");
            }
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _agent.Title = txtTitle.Text;
            _agent.AgentType = cmbAgentType.SelectedItem as AgentType;
            _agent.Address = txtAdress.Text;
            _agent.INN = txtINN.Text;
            _agent.KPP = txtKPP.Text;
            _agent.DirectorName = txtDirectorName.Text;
            _agent.Phone = txtPhone.Text;
            _agent.Email = txtEmail.Text;
            string path = cmbLogo.Text.Replace("../Icons", "");
            _agent.Logo = path;
            _agent.Priority = priority;
            if (_db.Agents.Find(_agent.ID) == null)
            {
                _db.Agents.Add(_agent);
            }
            try
            {
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("Успешно сохранено!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
            this.NavigationService.GoBack();
        }
    }
}
