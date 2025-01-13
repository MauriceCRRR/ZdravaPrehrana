using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZdravaPrehrana.Migrations
{
    /// <inheritdoc />
    public partial class DodajanjeDeljenjaMenijev : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sestavine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Naziv = table.Column<string>(type: "TEXT", nullable: false),
                    Kalorije = table.Column<double>(type: "REAL", nullable: false),
                    Beljakovine = table.Column<double>(type: "REAL", nullable: false),
                    Mascobe = table.Column<double>(type: "REAL", nullable: false),
                    OgljikoviHidrati = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sestavine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Uporabniki",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UporabniskoIme = table.Column<string>(type: "TEXT", nullable: false),
                    Geslo = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Vloga = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uporabniki", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jedilniki",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Naziv = table.Column<string>(type: "TEXT", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "TEXT", nullable: false),
                    JeDeljiv = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DatumDeljenja = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UporabnikId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jedilniki", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jedilniki_Uporabniki_UporabnikId",
                        column: x => x.UporabnikId,
                        principalTable: "Uporabniki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NakupovalniSeznami",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Naziv = table.Column<string>(type: "TEXT", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UporabnikId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NakupovalniSeznami", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NakupovalniSeznami_Uporabniki_UporabnikId",
                        column: x => x.UporabnikId,
                        principalTable: "Uporabniki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Nasveti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Vprasanje = table.Column<string>(type: "TEXT", nullable: false),
                    Odgovor = table.Column<string>(type: "TEXT", nullable: true),
                    DatumVprasanja = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DatumOdgovora = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    UporabnikId = table.Column<int>(type: "INTEGER", nullable: false),
                    StrokovnjakId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nasveti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nasveti_Uporabniki_StrokovnjakId",
                        column: x => x.StrokovnjakId,
                        principalTable: "Uporabniki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Nasveti_Uporabniki_UporabnikId",
                        column: x => x.UporabnikId,
                        principalTable: "Uporabniki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrehranskiCilji",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CiljnaTeza = table.Column<double>(type: "REAL", nullable: false),
                    CasovniOkvir = table.Column<int>(type: "INTEGER", nullable: false),
                    DnevneKalorije = table.Column<int>(type: "INTEGER", nullable: false),
                    UporabnikId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrehranskiCilji", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrehranskiCilji_Uporabniki_UporabnikId",
                        column: x => x.UporabnikId,
                        principalTable: "Uporabniki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Profili",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UporabnikId = table.Column<int>(type: "INTEGER", nullable: false),
                    Visina = table.Column<double>(type: "REAL", nullable: false),
                    Teza = table.Column<double>(type: "REAL", nullable: false),
                    Alergije = table.Column<string>(type: "TEXT", nullable: false),
                    Omejitve = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profili", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profili_Uporabniki_UporabnikId",
                        column: x => x.UporabnikId,
                        principalTable: "Uporabniki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VnosiHranil",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Datum = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Kalorije = table.Column<int>(type: "INTEGER", nullable: false),
                    Beljakovine = table.Column<double>(type: "REAL", nullable: false),
                    Mascobe = table.Column<double>(type: "REAL", nullable: false),
                    OgljikoviHidrati = table.Column<double>(type: "REAL", nullable: false),
                    UporabnikId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VnosiHranil", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VnosiHranil_Uporabniki_UporabnikId",
                        column: x => x.UporabnikId,
                        principalTable: "Uporabniki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeljeniJedilniki",
                columns: table => new
                {
                    DeliZId = table.Column<int>(type: "INTEGER", nullable: false),
                    JedilnikId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeljeniJedilniki", x => new { x.DeliZId, x.JedilnikId });
                    table.ForeignKey(
                        name: "FK_DeljeniJedilniki_Jedilniki_JedilnikId",
                        column: x => x.JedilnikId,
                        principalTable: "Jedilniki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeljeniJedilniki_Uporabniki_DeliZId",
                        column: x => x.DeliZId,
                        principalTable: "Uporabniki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JedilnikOcene",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Vrednost = table.Column<int>(type: "INTEGER", nullable: false),
                    Komentar = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DatumOcene = table.Column<DateTime>(type: "TEXT", nullable: false),
                    JedilnikId = table.Column<int>(type: "INTEGER", nullable: false),
                    UporabnikId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JedilnikOcene", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JedilnikOcene_Jedilniki_JedilnikId",
                        column: x => x.JedilnikId,
                        principalTable: "Jedilniki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JedilnikOcene_Uporabniki_UporabnikId",
                        column: x => x.UporabnikId,
                        principalTable: "Uporabniki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Obroki",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Naziv = table.Column<string>(type: "TEXT", nullable: false),
                    Cas = table.Column<DateTime>(type: "TEXT", nullable: false),
                    JedilnikId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obroki", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Obroki_Jedilniki_JedilnikId",
                        column: x => x.JedilnikId,
                        principalTable: "Jedilniki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recepti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Naziv = table.Column<string>(type: "TEXT", nullable: false),
                    Postopek = table.Column<string>(type: "TEXT", nullable: false),
                    Kalorije = table.Column<int>(type: "INTEGER", nullable: false),
                    CasPriprave = table.Column<int>(type: "INTEGER", nullable: false),
                    JedilnikId = table.Column<int>(type: "INTEGER", nullable: true),
                    AvtorId = table.Column<int>(type: "INTEGER", nullable: false),
                    JeJaven = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DatumUstvarjanja = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValue: new DateTime(2025, 1, 13, 12, 10, 18, 692, DateTimeKind.Local).AddTicks(3948))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recepti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recepti_Jedilniki_JedilnikId",
                        column: x => x.JedilnikId,
                        principalTable: "Jedilniki",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Recepti_Uporabniki_AvtorId",
                        column: x => x.AvtorId,
                        principalTable: "Uporabniki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SeznamPostavke",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Naziv = table.Column<string>(type: "TEXT", nullable: false),
                    Kolicina = table.Column<double>(type: "REAL", nullable: false),
                    Enota = table.Column<string>(type: "TEXT", nullable: false),
                    JeObkljukana = table.Column<bool>(type: "INTEGER", nullable: false),
                    NakupovalniSeznamId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeznamPostavke", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeznamPostavke_NakupovalniSeznami_NakupovalniSeznamId",
                        column: x => x.NakupovalniSeznamId,
                        principalTable: "NakupovalniSeznami",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ocene",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Vrednost = table.Column<int>(type: "INTEGER", nullable: false),
                    Komentar = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DatumOcene = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReceptId = table.Column<int>(type: "INTEGER", nullable: false),
                    UporabnikId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ocene", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ocene_Recepti_ReceptId",
                        column: x => x.ReceptId,
                        principalTable: "Recepti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ocene_Uporabniki_UporabnikId",
                        column: x => x.UporabnikId,
                        principalTable: "Uporabniki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReceptiObroki",
                columns: table => new
                {
                    ObrokiId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReceptiId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceptiObroki", x => new { x.ObrokiId, x.ReceptiId });
                    table.ForeignKey(
                        name: "FK_ReceptiObroki_Obroki_ObrokiId",
                        column: x => x.ObrokiId,
                        principalTable: "Obroki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceptiObroki_Recepti_ReceptiId",
                        column: x => x.ReceptiId,
                        principalTable: "Recepti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceptSestavine",
                columns: table => new
                {
                    ReceptId = table.Column<int>(type: "INTEGER", nullable: false),
                    SestavinaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Kolicina = table.Column<double>(type: "REAL", nullable: false),
                    Enota = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceptSestavine", x => new { x.ReceptId, x.SestavinaId });
                    table.ForeignKey(
                        name: "FK_ReceptSestavine_Recepti_ReceptId",
                        column: x => x.ReceptId,
                        principalTable: "Recepti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceptSestavine_Sestavine_SestavinaId",
                        column: x => x.SestavinaId,
                        principalTable: "Sestavine",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeljeniJedilniki_JedilnikId",
                table: "DeljeniJedilniki",
                column: "JedilnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Jedilniki_UporabnikId",
                table: "Jedilniki",
                column: "UporabnikId");

            migrationBuilder.CreateIndex(
                name: "IX_JedilnikOcene_JedilnikId",
                table: "JedilnikOcene",
                column: "JedilnikId");

            migrationBuilder.CreateIndex(
                name: "IX_JedilnikOcene_UporabnikId",
                table: "JedilnikOcene",
                column: "UporabnikId");

            migrationBuilder.CreateIndex(
                name: "IX_NakupovalniSeznami_UporabnikId",
                table: "NakupovalniSeznami",
                column: "UporabnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Nasveti_StrokovnjakId",
                table: "Nasveti",
                column: "StrokovnjakId");

            migrationBuilder.CreateIndex(
                name: "IX_Nasveti_UporabnikId",
                table: "Nasveti",
                column: "UporabnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Obroki_JedilnikId",
                table: "Obroki",
                column: "JedilnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Ocene_ReceptId",
                table: "Ocene",
                column: "ReceptId");

            migrationBuilder.CreateIndex(
                name: "IX_Ocene_UporabnikId",
                table: "Ocene",
                column: "UporabnikId");

            migrationBuilder.CreateIndex(
                name: "IX_PrehranskiCilji_UporabnikId",
                table: "PrehranskiCilji",
                column: "UporabnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Profili_UporabnikId",
                table: "Profili",
                column: "UporabnikId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recepti_AvtorId",
                table: "Recepti",
                column: "AvtorId");

            migrationBuilder.CreateIndex(
                name: "IX_Recepti_JedilnikId",
                table: "Recepti",
                column: "JedilnikId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceptiObroki_ReceptiId",
                table: "ReceptiObroki",
                column: "ReceptiId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceptSestavine_SestavinaId",
                table: "ReceptSestavine",
                column: "SestavinaId");

            migrationBuilder.CreateIndex(
                name: "IX_SeznamPostavke_NakupovalniSeznamId",
                table: "SeznamPostavke",
                column: "NakupovalniSeznamId");

            migrationBuilder.CreateIndex(
                name: "IX_VnosiHranil_UporabnikId",
                table: "VnosiHranil",
                column: "UporabnikId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeljeniJedilniki");

            migrationBuilder.DropTable(
                name: "JedilnikOcene");

            migrationBuilder.DropTable(
                name: "Nasveti");

            migrationBuilder.DropTable(
                name: "Ocene");

            migrationBuilder.DropTable(
                name: "PrehranskiCilji");

            migrationBuilder.DropTable(
                name: "Profili");

            migrationBuilder.DropTable(
                name: "ReceptiObroki");

            migrationBuilder.DropTable(
                name: "ReceptSestavine");

            migrationBuilder.DropTable(
                name: "SeznamPostavke");

            migrationBuilder.DropTable(
                name: "VnosiHranil");

            migrationBuilder.DropTable(
                name: "Obroki");

            migrationBuilder.DropTable(
                name: "Recepti");

            migrationBuilder.DropTable(
                name: "Sestavine");

            migrationBuilder.DropTable(
                name: "NakupovalniSeznami");

            migrationBuilder.DropTable(
                name: "Jedilniki");

            migrationBuilder.DropTable(
                name: "Uporabniki");
        }
    }
}
