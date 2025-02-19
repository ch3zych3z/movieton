﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MovieTon.Database;

#nullable disable

namespace MovieTon.Database.Migrations
{
    [DbContext(typeof(MovieDbContext))]
    partial class MovieDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("MovieTon.Database.DbMovie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("MovieTon.Database.DbMovieTag", b =>
                {
                    b.Property<int>("TagId")
                        .HasColumnType("int");

                    b.Property<int>("MovieId")
                        .HasColumnType("int");

                    b.HasKey("TagId", "MovieId");

                    b.HasIndex("MovieId");

                    b.ToTable("MovieTags");
                });

            modelBuilder.Entity("MovieTon.Database.DbParticipation", b =>
                {
                    b.Property<string>("Role")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("StaffId")
                        .HasColumnType("int");

                    b.Property<int>("MovieId")
                        .HasColumnType("int");

                    b.HasKey("Role", "StaffId", "MovieId");

                    b.HasIndex("MovieId");

                    b.HasIndex("StaffId");

                    b.ToTable("Participation");
                });

            modelBuilder.Entity("MovieTon.Database.DbSimilarity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("Confidence")
                        .HasColumnType("double");

                    b.Property<int>("SimilarFromId")
                        .HasColumnType("int");

                    b.Property<int>("SimilarToId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SimilarFromId");

                    b.HasIndex("SimilarToId");

                    b.ToTable("Similarities");
                });

            modelBuilder.Entity("MovieTon.Database.DbStaffMember", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("StaffMembers");
                });

            modelBuilder.Entity("MovieTon.Database.DbTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("MovieTon.Database.DbTitle", b =>
                {
                    b.Property<string>("Title")
                        .HasColumnType("varchar(255)");

                    MySqlPropertyBuilderExtensions.HasCharSet(b.Property<string>("Title"), "utf8");

                    b.Property<int>("MovieId")
                        .HasColumnType("int");

                    b.Property<string>("Local")
                        .HasMaxLength(2)
                        .HasColumnType("varchar(2)");

                    b.HasKey("Title", "MovieId", "Local");

                    b.HasIndex("MovieId");

                    b.ToTable("Titles");
                });

            modelBuilder.Entity("MovieTon.Database.DbMovieTag", b =>
                {
                    b.HasOne("MovieTon.Database.DbMovie", "Movie")
                        .WithMany("MovieTags")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieTon.Database.DbTag", "Tag")
                        .WithMany("MovieTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("MovieTon.Database.DbParticipation", b =>
                {
                    b.HasOne("MovieTon.Database.DbMovie", "Movie")
                        .WithMany("Participation")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieTon.Database.DbStaffMember", "StaffMember")
                        .WithMany("Participation")
                        .HasForeignKey("StaffId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");

                    b.Navigation("StaffMember");
                });

            modelBuilder.Entity("MovieTon.Database.DbSimilarity", b =>
                {
                    b.HasOne("MovieTon.Database.DbMovie", "SimilarFrom")
                        .WithMany("SimilaritiesFrom")
                        .HasForeignKey("SimilarFromId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieTon.Database.DbMovie", "SimilarTo")
                        .WithMany("SimilaritiesTo")
                        .HasForeignKey("SimilarToId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SimilarFrom");

                    b.Navigation("SimilarTo");
                });

            modelBuilder.Entity("MovieTon.Database.DbTitle", b =>
                {
                    b.HasOne("MovieTon.Database.DbMovie", "Movie")
                        .WithMany("Titles")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("MovieTon.Database.DbMovie", b =>
                {
                    b.Navigation("MovieTags");

                    b.Navigation("Participation");

                    b.Navigation("SimilaritiesFrom");

                    b.Navigation("SimilaritiesTo");

                    b.Navigation("Titles");
                });

            modelBuilder.Entity("MovieTon.Database.DbStaffMember", b =>
                {
                    b.Navigation("Participation");
                });

            modelBuilder.Entity("MovieTon.Database.DbTag", b =>
                {
                    b.Navigation("MovieTags");
                });
#pragma warning restore 612, 618
        }
    }
}
