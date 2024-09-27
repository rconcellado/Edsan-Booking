using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EdsanBooking.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "checkin",
                columns: table => new
                {
                    checkinid = table.Column<string>(type: "text", nullable: false),
                    loc = table.Column<string>(type: "text", nullable: false),
                    guestid = table.Column<string>(type: "text", nullable: false),
                    numguest = table.Column<int>(type: "integer", nullable: false),
                    checkindt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    checkintime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    checkoutdt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    checkouttime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    remarks = table.Column<string>(type: "text", nullable: false),
                    reservationid = table.Column<string>(type: "text", nullable: false),
                    checkintype = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    createdby = table.Column<string>(type: "text", nullable: false),
                    createddt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkin", x => x.checkinid);
                });

            migrationBuilder.CreateTable(
                name: "comreserved",
                columns: table => new
                {
                    comresid = table.Column<string>(type: "text", nullable: false),
                    guestid = table.Column<string>(type: "text", nullable: false),
                    reservationid = table.Column<string>(type: "text", nullable: false),
                    preference = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comreserved", x => x.comresid);
                });

            migrationBuilder.CreateTable(
                name: "guest",
                columns: table => new
                {
                    guestid = table.Column<string>(type: "text", nullable: false),
                    company = table.Column<string>(type: "text", nullable: false),
                    gname = table.Column<string>(type: "text", nullable: false),
                    lname = table.Column<string>(type: "text", nullable: false),
                    contactno = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    address = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    guesttype = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guest", x => x.guestid);
                });

            migrationBuilder.CreateTable(
                name: "paymenthistory",
                columns: table => new
                {
                    transactionid = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    transactiondate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    reservationid = table.Column<string>(type: "text", nullable: false),
                    checkinid = table.Column<string>(type: "text", nullable: false),
                    transactiontype = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paymenthistory", x => x.transactionid);
                });

            migrationBuilder.CreateTable(
                name: "reservation",
                columns: table => new
                {
                    reservationid = table.Column<string>(type: "text", nullable: false),
                    loc = table.Column<string>(type: "text", nullable: false),
                    guestid = table.Column<string>(type: "text", nullable: false),
                    numguest = table.Column<int>(type: "integer", nullable: false),
                    checkindt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    checkintime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    checkoutdt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    checkouttime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    remarks = table.Column<string>(type: "text", nullable: false),
                    reservationtype = table.Column<string>(type: "text", nullable: false),
                    createdby = table.Column<string>(type: "text", nullable: false),
                    createddt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservation", x => x.reservationid);
                });

            migrationBuilder.CreateTable(
                name: "resortamenities",
                columns: table => new
                {
                    amenityid = table.Column<string>(type: "text", nullable: false),
                    pkgname = table.Column<string>(type: "text", nullable: false),
                    amenity = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resortamenities", x => x.amenityid);
                });

            migrationBuilder.CreateTable(
                name: "resortpkg",
                columns: table => new
                {
                    packageid = table.Column<string>(type: "text", nullable: false),
                    descr = table.Column<string>(type: "text", nullable: false),
                    packageamt = table.Column<decimal>(type: "numeric", nullable: false),
                    hourtype = table.Column<int>(type: "integer", nullable: false),
                    numroom = table.Column<int>(type: "integer", nullable: false),
                    loc = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resortpkg", x => x.packageid);
                });

            migrationBuilder.CreateTable(
                name: "resortres",
                columns: table => new
                {
                    resortresid = table.Column<string>(type: "text", nullable: false),
                    pkgname = table.Column<string>(type: "text", nullable: false),
                    reservationid = table.Column<string>(type: "text", nullable: false),
                    checkinid = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resortres", x => x.resortresid);
                });

            migrationBuilder.CreateTable(
                name: "room",
                columns: table => new
                {
                    roomid = table.Column<string>(type: "text", nullable: false),
                    descr = table.Column<string>(type: "text", nullable: false),
                    featurename = table.Column<string>(type: "text", nullable: false),
                    typename = table.Column<string>(type: "text", nullable: false),
                    hourtype = table.Column<int>(type: "integer", nullable: false),
                    statusname = table.Column<string>(type: "text", nullable: false),
                    classification = table.Column<string>(type: "text", nullable: false),
                    remarks = table.Column<string>(type: "text", nullable: false),
                    loc = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room", x => x.roomid);
                });

            migrationBuilder.CreateTable(
                name: "roomcheckin",
                columns: table => new
                {
                    roomcheckinid = table.Column<string>(type: "text", nullable: false),
                    checkinid = table.Column<string>(type: "text", nullable: false),
                    guestid = table.Column<string>(type: "text", nullable: false),
                    roomid = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roomcheckin", x => x.roomcheckinid);
                });

            migrationBuilder.CreateTable(
                name: "roomfeature",
                columns: table => new
                {
                    roomfeatureid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    featurename = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roomfeature", x => x.roomfeatureid);
                });

            migrationBuilder.CreateTable(
                name: "roomimages",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    featurename = table.Column<string>(type: "text", nullable: false),
                    typename = table.Column<string>(type: "text", nullable: false),
                    imagepath1 = table.Column<string>(type: "text", nullable: false),
                    imagepath2 = table.Column<string>(type: "text", nullable: false),
                    imagepath3 = table.Column<string>(type: "text", nullable: false),
                    imagepath4 = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roomimages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roomrate",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    featurename = table.Column<string>(type: "text", nullable: false),
                    typename = table.Column<string>(type: "text", nullable: false),
                    hourtype = table.Column<int>(type: "integer", nullable: false),
                    roomrate = table.Column<decimal>(type: "numeric", nullable: false),
                    loc = table.Column<string>(type: "text", nullable: false),
                    classification = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roomrate", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roomtype",
                columns: table => new
                {
                    roomtypeid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    typename = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roomtype", x => x.roomtypeid);
                });

            migrationBuilder.CreateTable(
                name: "SPRoomLoad",
                columns: table => new
                {
                    roomid = table.Column<string>(type: "text", nullable: false),
                    descr = table.Column<string>(type: "text", nullable: false),
                    roomfeatureid = table.Column<int>(type: "integer", nullable: false),
                    featurename = table.Column<string>(type: "text", nullable: false),
                    roomtypeid = table.Column<int>(type: "integer", nullable: false),
                    typename = table.Column<string>(type: "text", nullable: false),
                    hourtype = table.Column<decimal>(type: "numeric", nullable: false),
                    statusid = table.Column<int>(type: "integer", nullable: false),
                    statusname = table.Column<string>(type: "text", nullable: false),
                    remarks = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPRoomLoad", x => x.roomid);
                });

            migrationBuilder.CreateTable(
                name: "status",
                columns: table => new
                {
                    statusid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    statusname = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_status", x => x.statusid);
                });

            migrationBuilder.CreateTable(
                name: "transientres",
                columns: table => new
                {
                    trresid = table.Column<string>(type: "text", nullable: false),
                    featurename = table.Column<string>(type: "text", nullable: false),
                    typename = table.Column<string>(type: "text", nullable: false),
                    hourtype = table.Column<int>(type: "integer", nullable: false),
                    reservationid = table.Column<string>(type: "text", nullable: false),
                    checkinid = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transientres", x => x.trresid);
                });

            migrationBuilder.CreateTable(
                name: "userguest",
                columns: table => new
                {
                    userid = table.Column<string>(type: "text", nullable: false),
                    guestid = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userguest", x => x.userid);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    userid = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    loc = table.Column<string>(type: "text", nullable: false),
                    failedloginattempts = table.Column<int>(type: "integer", nullable: false),
                    islocked = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    secquest = table.Column<string>(type: "text", nullable: false),
                    secanswer = table.Column<string>(type: "text", nullable: false),
                    userrole = table.Column<string>(type: "text", nullable: false),
                    createdby = table.Column<string>(type: "text", nullable: false),
                    createddt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.userid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "checkin");

            migrationBuilder.DropTable(
                name: "comreserved");

            migrationBuilder.DropTable(
                name: "guest");

            migrationBuilder.DropTable(
                name: "paymenthistory");

            migrationBuilder.DropTable(
                name: "reservation");

            migrationBuilder.DropTable(
                name: "resortamenities");

            migrationBuilder.DropTable(
                name: "resortpkg");

            migrationBuilder.DropTable(
                name: "resortres");

            migrationBuilder.DropTable(
                name: "room");

            migrationBuilder.DropTable(
                name: "roomcheckin");

            migrationBuilder.DropTable(
                name: "roomfeature");

            migrationBuilder.DropTable(
                name: "roomimages");

            migrationBuilder.DropTable(
                name: "roomrate");

            migrationBuilder.DropTable(
                name: "roomtype");

            migrationBuilder.DropTable(
                name: "SPRoomLoad");

            migrationBuilder.DropTable(
                name: "status");

            migrationBuilder.DropTable(
                name: "transientres");

            migrationBuilder.DropTable(
                name: "userguest");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
