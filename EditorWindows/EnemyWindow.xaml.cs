using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using BeyondVisionEngine.Components;

namespace BeyondVisionEngine.EditorWindows
{
    /// <summary>
    /// Interaction logic for EnemyWindow.xaml
    /// </summary>
    public partial class EnemyWindow : Window
    {
        private Enemy _enemy;
        public EnemyWindow(Enemy enemy)
        {
            InitializeComponent();
            _enemy = enemy;

            if (_enemy.Hp != 0)
                HpBox.Text = _enemy.Hp.ToString();

            if (_enemy.Attack != 0)
                AttackBox.Text = _enemy.Attack.ToString();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(AttackBox.Text, out var tmpAttack) || AttackBox.Text == "")
            {
                MessageBox.Show("Attack should contain only numeric values, and can't be null!");
                return;
            }
            if (!int.TryParse(HpBox.Text, out var tmpHp) || HpBox.Text == "")
            {
                MessageBox.Show("Hp should contain only numeric value, and can't be null!");
                return;
            }

            _enemy.Attack = tmpAttack;
            _enemy.Hp = tmpHp;

            Close();
        }
    }
}
