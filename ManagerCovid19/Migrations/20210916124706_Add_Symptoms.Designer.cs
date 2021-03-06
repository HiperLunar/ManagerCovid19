// <auto-generated />
using System;
using ManagerCovid19.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ManagerCovid19.Migrations
{
    [DbContext(typeof(ManagerCovid19Context))]
    [Migration("20210916124706_Add_Symptoms")]
    partial class Add_Symptoms
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ManagerCovid19.Models.HealthRegistration", b =>
                {
                    b.Property<int>("HealthRegistrationID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("HowRUFeeling")
                        .HasColumnType("bit");

                    b.Property<string>("MemberRegistrationNumber")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("RegisterDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Symptoms")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("HealthRegistrationID");

                    b.HasIndex("MemberRegistrationNumber");

                    b.ToTable("HealthRegistration");
                });

            modelBuilder.Entity("ManagerCovid19.Models.Member", b =>
                {
                    b.Property<string>("MemberRegistrationNumber")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("Admin")
                        .HasColumnType("bit");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("Date");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sector")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MemberRegistrationNumber");

                    b.ToTable("Member");
                });

            modelBuilder.Entity("ManagerCovid19.Models.HealthRegistration", b =>
                {
                    b.HasOne("ManagerCovid19.Models.Member", "Member")
                        .WithMany("HealthRegistrations")
                        .HasForeignKey("MemberRegistrationNumber");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("ManagerCovid19.Models.Member", b =>
                {
                    b.Navigation("HealthRegistrations");
                });
#pragma warning restore 612, 618
        }
    }
}
