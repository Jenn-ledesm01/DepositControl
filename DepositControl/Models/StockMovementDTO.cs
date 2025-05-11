using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration; // Necesario para acceder a ConfigurationManager

namespace DepositControl.Bussines
{
    public class StockMovementDTO
    {
        public int StockMovement_Id { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public int Product_Id { get; set; }
        public string Product_Name { get; set; }
        public string Origen { get; set; }
        public int Stock_Id { get; set; }
        public int User_Id { get; set; }
        public string User_FullName { get; set; }
        public int Quantity { get; set; }

        public string DetalleURL { get; set; }

        public int? PurchaseOrder_Id { get; set; }
        public int? DeliveryNote_Id { get; set; }

        public string DeliveryNote_Number { get; set; }

        public string PurchaseOrder_Number { get; set; }
        public List<StockMovementDTO> GetStockMovement()
        {
            List<StockMovementDTO> stockMovements = new List<StockMovementDTO>();

            string connectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("StockMovementDTO_Sp_GetStockMovement", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    stockMovements.Add(new StockMovementDTO
                    {
                        StockMovement_Id = reader["StockMovement_Id"] as int? ?? 0,
                        FechaMovimiento = reader["FechaMovimiento"] as DateTime? ?? DateTime.MinValue,
                        Product_Id = reader["Product_Id"] as int? ?? 0,
                        Product_Name = reader["Product_Name"] as string,
                        Origen = reader["Origen"] as string,
                        Stock_Id = reader["Stock_Id"] as int? ?? 0,
                        User_Id = reader["User_Id"] as int? ?? 0,
                        User_FullName = reader["User_FullName"] as string,
                        Quantity = reader["Quantity"] as int? ?? 0,
                        PurchaseOrder_Id = reader["PurchaseOrder_Id"] as int?,
                        DeliveryNote_Id = reader["DeliveryNote_Id"] as int?,
                        PurchaseOrder_Number = reader["PurchaseOrder_Number"] as string,
                        DeliveryNote_Number = reader["DeliveryNote_Number"] as string
                    });
                }

                reader.Close();
            }

            return stockMovements;
        }

        public List<StockMovementDTO> GetStockMovementByFilter(string productName, string tipoMovimiento, string numero)
        {
            List<StockMovementDTO> stockMovements = new List<StockMovementDTO>();

            string connectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("StockMovementDTO_Sp_GetByFilter", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ProductName", string.IsNullOrEmpty(productName) ? (object)DBNull.Value : productName);
                cmd.Parameters.AddWithValue("@TipoMovimiento", string.IsNullOrEmpty(tipoMovimiento) ? (object)DBNull.Value : tipoMovimiento);
                cmd.Parameters.AddWithValue("@Numero", string.IsNullOrEmpty(numero) ? (object)DBNull.Value : numero);

                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    stockMovements.Add(new StockMovementDTO
                    {
                        StockMovement_Id = reader["StockMovement_Id"] as int? ?? 0,
                        FechaMovimiento = reader["FechaMovimiento"] as DateTime? ?? DateTime.MinValue,
                        Product_Id = reader["Product_Id"] as int? ?? 0,
                        Product_Name = reader["Product_Name"] as string,
                        Origen = reader["Origen"] as string,
                        Stock_Id = reader["Stock_Id"] as int? ?? 0,
                        User_Id = reader["User_Id"] as int? ?? 0,
                        User_FullName = reader["User_FullName"] as string,
                        Quantity = reader["Quantity"] as int? ?? 0,
                        PurchaseOrder_Id = reader["PurchaseOrder_Id"] as int?,
                        DeliveryNote_Id = reader["DeliveryNote_Id"] as int?,
                        PurchaseOrder_Number = reader["PurchaseOrder_Number"] as string,
                        DeliveryNote_Number = reader["DeliveryNote_Number"] as string
                    });
                }

                reader.Close();
            }

            return stockMovements;
        }
    }
}
