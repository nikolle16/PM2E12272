using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PM2E12272.Models;

namespace PM2E12272.Controllers
{
    internal class UbicacionControllers
    {
        SQLiteAsyncConnection _connection;

        //Constructor vacio
        public UbicacionControllers() { }

        //Conexion a la base de datos
        public async Task Init()
        {
            try
            {
                if (_connection is null)
                {
                    SQLite.SQLiteOpenFlags extensiones = SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create | SQLite.SQLiteOpenFlags.SharedCache;

                    if (string.IsNullOrEmpty(FileSystem.AppDataDirectory))
                    {
                        return;
                    }

                    _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "DBUbicacion"), extensiones);

                    var creacion = await _connection.CreateTableAsync<Models.Ubicacion>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Init(): {ex.Message}");
            }
        }

        //Crear metodos crud para la clase personas
        //Create
        public async Task<int> storeAutor(Ubicacion ubicacion)
        {
            await Init();
            if (ubicacion.Id == 0)
            {
                return await _connection.InsertAsync(ubicacion);
            }
            else
            {
                return await _connection.UpdateAsync(ubicacion);
            }
        }

        //Update
        public async Task<int> updateAutor(Ubicacion ubicacion)
        {
            await Init();
            return await _connection.UpdateAsync(ubicacion);
        }

        //Read
        public async Task<List<Models.Ubicacion>> getListAutor()
        {
            await Init();
            return await _connection.Table<Ubicacion>().ToListAsync();
        }

        //Read Element
        public async Task<Models.Ubicacion> getAutors(int pid)
        {
            await Init();
            return await _connection.Table<Ubicacion>().Where(i => i.Id == pid).FirstOrDefaultAsync();
        }

        //Delete
        public async Task<int> deleteAutor(int ubicacionID)
        {
            await Init();
            var ubicacionToDelete = await getAutors(ubicacionID);

            if (ubicacionToDelete != null)
            {
                return await _connection.DeleteAsync(ubicacionToDelete);
            }

            return 0;
        }
    }
}
