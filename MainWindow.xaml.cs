using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
namespace WpfApp1
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public System.Threading.Thread Thread { get; set; }
        public string Host = "http://MyIpAddressOfServer:8089/signalr";

        public bool Active { get; set; }
        public HubConnection connection;
        public MySqlDataAdapter ad;
        public MySqlDataAdapter adlistview_paneles;
        public MySqlDataAdapter adlistview_rotaciones;
        public MySqlDataAdapter adGridOrden;
        public DataSet ds;
        public DataSet dslistviewpaneles;
        public DataSet dslistviewrotaciones;
        public DataSet dsGridOrden;

        public MainWindow()
        {
            InitializeComponent();


            connection = new HubConnectionBuilder().WithUrl("http://192.168.0.94:5000/chatHub").Build();


            string connectionString = "datasource=192.168.0.94;port=3306;username=apppruebas;password=Capeluam209173$$_;database=test;";
            // Tu consulta en SQL
            string query = "SET sql_mode='';SET @rank=0;SELECT @rank:=@rank+1 AS rank, scores.* from (select Dorsal,E1,E2,E3,E4,(e1+e2+e3+e4-COALESCE(GREATEST(E1,E2,E3,E4))-COALESCE(least(E1,E2,E3,E4)))/2 as E,A1,A2,A3,A4,(a1+a2+a3+a4-COALESCE(GREATEST(a1,a2,a3,a4))-COALESCE(least(a1,a2,a3,a4)))/2 as A,D,P, D+((a1+a2+a3+a4-COALESCE(GREATEST(a1,a2,a3,a4))-COALESCE(least(a1,a2,a3,a4)))/2)+((e1+e2+e3+e4-COALESCE(GREATEST(E1,E2,E3,E4))-COALESCE(least(E1,E2,E3,E4)))/2)-P as SCORE,((a1+a2+a3+a4-COALESCE(GREATEST(a1,a2,a3,a4))-COALESCE(least(a1,a2,a3,a4)))/2)+((e1+e2+e3+e4-COALESCE(GREATEST(E1,E2,E3,E4))-COALESCE(least(E1,E2,E3,E4)))/2) as AE FROM SCORE group by Dorsal order by score DESC, e desc, ae DESC) scores";
            string query2 = "SELECT * FROM Gimnastas";
            string query3 = "obtener_ranking";
            string query4 = "obtener_paneles";
            string query5 = "obtener_rotaciones";
            string query6 = "SELECT OrdenCompeticion.`Orden`, Gimnastas.`Dorsal`, Gimnastas, categoria, modalidad, `Panel`, `Rotacion`, `Ejercicio` FROM `OrdenCompeticion` left join Gimnastas on Gimnastas.dorsal=OrdenCompeticion.dorsal WHERE Rotacion=1 and OrdenCompeticion.Dorsal in ( SELECT SCORE.Dorsal FROM `SCORE` WHERE EnableScore=0 and A1+A2+A3+A4=0)";
            // Prepara la conexión
            ad = new MySqlDataAdapter();
            adlistview_paneles= new MySqlDataAdapter();
            adlistview_rotaciones = new MySqlDataAdapter();
            adGridOrden = new MySqlDataAdapter();
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabaseGridScore = new MySqlCommand(query3, databaseConnection);
            ad.SelectCommand = commandDatabaseGridScore;
            commandDatabaseGridScore.CommandTimeout = 60;

            MySqlCommand commandDatabaselistview_paneles = new MySqlCommand(query4, databaseConnection);
            adlistview_paneles.SelectCommand = commandDatabaselistview_paneles;
            commandDatabaselistview_paneles.CommandTimeout = 60;

            MySqlCommand commandDatabaseGridScorelistview_rotaciones = new MySqlCommand(query5, databaseConnection);
            adlistview_rotaciones.SelectCommand = commandDatabaseGridScorelistview_rotaciones;
            commandDatabaseGridScore.CommandTimeout = 60;

            MySqlCommand commandDatabaseGridOrden = new MySqlCommand(query6, databaseConnection);
            adGridOrden.SelectCommand = commandDatabaseGridOrden;
            commandDatabaseGridOrden.CommandTimeout = 60;


            // Consulta datos Grid Score
            try
            {
                // Abre la base de datos
                databaseConnection.Open();
                ds = new DataSet();
                ad.Fill(ds);
                
                GridScore.DataContext= ds.Tables[0].DefaultView;
                
                dslistviewpaneles = new DataSet();
                adlistview_paneles.Fill(dslistviewpaneles);
                ComboBoxPanel.DataContext= dslistviewpaneles.Tables[0].DefaultView;
               
                dslistviewrotaciones = new DataSet();
                adlistview_rotaciones.Fill(dslistviewrotaciones);
                ComboBoxRotacion.DataContext = dslistviewrotaciones.Tables[0].DefaultView;

                dsGridOrden = new DataSet();
                adGridOrden.Fill(dsGridOrden);
                GridOrden.DataContext = dsGridOrden.Tables[0].DefaultView;


                // Cerrar la conexión
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                // Mostrar cualquier excepción
                MessageBox.Show(ex.Message);
            }
            DataGrid dataGrid_C1 = new DataGrid();
            dataGrid_C1.AutoGenerateColumns = true;
            //dataGrid.Columns.Add(CreateTextBoxColumn());
            //dataGrid.Columns.Add(CreateComboBoxColumn());

            //set DataTable as item source of dataGrid
            // dataGrid.ItemsSource = GetDataTable().AsDataView();
            dataGrid_C1.ItemsSource = ds.Tables[0].DefaultView;
            //place DataGrid inside main Grid
            //Grid.SetColumn(dataGrid, 20);

           //Grid.SetRow(dataGrid, 20);

            GridClasificaciones_1.Children.Add(dataGrid_C1);

            DataGrid dataGrid_C2 = new DataGrid();
            dataGrid_C1.AutoGenerateColumns = true;
            //dataGrid.Columns.Add(CreateTextBoxColumn());
            //dataGrid.Columns.Add(CreateComboBoxColumn());

            //set DataTable as item source of dataGrid
            // dataGrid.ItemsSource = GetDataTable().AsDataView();
            dataGrid_C2.ItemsSource = ds.Tables[0].DefaultView;
            //place DataGrid inside main Grid
            //Grid.SetColumn(dataGrid, 20);

            //Grid.SetRow(dataGrid, 20);

           // GridClasificaciones_2.Children.Add(dataGrid_C2);
            
        }
        

        private async void Conectar(object sender, RoutedEventArgs e)
        {
            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{user}: {message}";
                    var nuevomensaje = message;
                    MessagesList.Items.Add(newMessage);
                    if (message == "Perfe")
                    {
                        MessagesList.Items.Add("ACTUALIZAR LA CONSULTA");
                        ds.Clear();
                        ad.Fill(ds);
                        GridScore.DataContext = ds.Tables[0].DefaultView;
                        ds.Dispose();
                        //ListViewScoreDetails.DataContext = ds.Tables[0].DefaultView;
                    }
                });
            });
            try
            {

                MessagesList.Items.Add("Conectando.....");
                await connection.StartAsync();
                MessagesList.Items.Add("Conexion establecida");
                MessagesList.Items.Add("Enviando mensaje.....");
                await connection.InvokeAsync("SendMessage",
                   "arosales", "Yo,hola,hola");

                //             connectButton.IsEnabled = false;
                //             sendButton.IsEnabled = true;

                connection.Closed += async (error) =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await connection.StartAsync();
                };

            }
            catch (Exception ex)
            {
                MessagesList.Items.Add(ex.Message);
            }

        }

    }
}
