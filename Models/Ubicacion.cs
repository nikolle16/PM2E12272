﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace PM2E12272.Models
{
    [Table("Ubicacion")]
    public class Ubicacion
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(250), NotNull]
        public string Latitud { get; set; } = string.Empty;

        [MaxLength(250), NotNull]
        public string Longitud { get; set; } = string.Empty;

        [MaxLength(250), NotNull]
        public string Descripcion { get; set; } = string.Empty;
        public string Foto { get; set; } = string.Empty;
    }
}
