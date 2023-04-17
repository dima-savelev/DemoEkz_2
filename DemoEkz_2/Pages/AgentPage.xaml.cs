using DemoEkz_2.Data;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для AgentPage.xaml
    /// </summary>
    public partial class AgentPage : Page
    {
        public readonly DbEntities _db = DbEntities.GetContext();
        public AgentPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _db.AgentTypes.Load();
            var list = _db.AgentTypes.Local.ToList();
            list.Insert(0, new AgentType() { Title = "Все типы" });
            cmbFiltr.ItemsSource = list;
            cmbFiltr.SelectedIndex = 0;
            cmbSotr.SelectedIndex= 0;
            _db.Agents.Load();
            var entities = _db.Agents.Local.ToBindingList();
            string defaultPath = "../Icons";
            DateTime yearBefore = DateTime.Now.AddYears(-1);
            //int countSales = 0;
            foreach (var item in entities)
            {
                item.CountSales = item.ProductSales.Where(p => p.SaleDate > yearBefore).Sum(p => p.ProductCount);
                decimal totalMoney = item.ProductSales.Sum(p => p.Product.MinCostForAgent * p.ProductCount);
                if (totalMoney < 10000)
                {
                    item.Discount = 0;
                }
                if (totalMoney >= 10000 && totalMoney < 50000)
                {
                    item.Discount = 5;
                }
                if (totalMoney >= 50000 && totalMoney < 150000)
                {
                    item.Discount = 10;
                }
                if (totalMoney >= 150000 && totalMoney < 500000)
                {
                    item.Discount = 20;
                }
                if (totalMoney >= 500000)
                {
                    item.Discount = 25;
                }
                //foreach (var sale in item.ProductSales)
                //{
                //    countSales += sale.ProductCount;
                //}
                if (string.IsNullOrEmpty(item.Logo))
                {
                    item.PathToIcon = defaultPath + "/picture.png";
                    continue;
                }
                item.PathToIcon = defaultPath + item.Logo;
            }
            listwiew.ItemsSource = entities;
            listwiew.Items.Refresh();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new AddEditAgentPage(new Agent()));
            
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Agent agent = btn.DataContext as Agent;
            _db.SaveChanges();
            this.NavigationService.Navigate(new AddEditAgentPage(agent));
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Agent agent = btn.DataContext as Agent;
            if (agent.ProductSales.Count != 0)
            {
                MessageBox.Show("Нельзя удалить агента, потому что он связан с реализацией продукции", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var answer = MessageBox.Show("Вы действительно хотите удалить эту запись?", "Предупреждение", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (answer != MessageBoxResult.Yes) return;
            _db.Agents.Remove(agent);
            _db.SaveChanges();
            Filter();
            MessageBox.Show("Запись удалена!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void cmbFiltr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void cmbSotr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void txtFind_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }
        private void Filter()
        {
            //Фильтрация
            var allEntities = _db.Agents.Local.ToList();
            AgentType type = cmbFiltr.SelectedItem as AgentType;
            if (type != null)
            {
                if (type.Title != "Все типы")
                {
                    allEntities = allEntities.Where(p => p.AgentType.ID == type.ID).ToList();
                }
            }
            //Поиск
            string findString = txtFind.Text;
            if (!string.IsNullOrEmpty(findString))
            {
                allEntities = allEntities.Where(p => p.Title.Contains(findString) || p.Phone.Contains(findString) || p.Email.Contains(findString)).ToList();
            }
            //Сортировка
            ComboBoxItem sort = cmbSotr.SelectedItem as ComboBoxItem;
            if (sort != null)
            {
                switch (sort.Content.ToString())
                {
                    case "Наименование по возростанию":
                        allEntities = allEntities.OrderBy(p => p.Title).ToList();
                        break;
                    case "Наименование по убыванию":
                        allEntities = allEntities.OrderByDescending(p => p.Title).ToList();
                        break;
                    case "Размер скидки по возрастанаию":
                        allEntities = allEntities.OrderBy(p => p.Discount).ToList();
                        break;
                    case "Размер скидки по убыванию":
                        allEntities = allEntities.OrderByDescending(p => p.Discount).ToList();
                        break;
                    case "Приоритет по возрастанаию":
                        allEntities = allEntities.OrderBy(p => p.Priority).ToList();
                        break;
                    case "Приоритет по убываниювы":
                        allEntities = allEntities.OrderByDescending(p => p.Priority).ToList();
                        break;
                }
            }
            listwiew.ItemsSource = allEntities;
        }
    }
}
