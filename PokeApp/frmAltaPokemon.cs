using dominio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using negocio;
using System.IO;
using System.Configuration;

namespace PokeApp
{
    
    public partial class frmAltaPokemon : Form
    {
        private Pokemon pokemon = null;
        private OpenFileDialog archivo = null;

        public frmAltaPokemon()
        {
            InitializeComponent();
        }

        public frmAltaPokemon(Pokemon pokemon)
        {
            InitializeComponent();
            this.pokemon = pokemon;
            Text = "Modificar Pokemon";
        }




        private void btnAceptar_Click(object sender, EventArgs e)
        {
            //Pokemon poke = new Pokemon(); AHORA USO EL ATRIBUTO PRIVADO POKEMON = NULL QUE CREE ANTES
            PokemonNegocio negocio = new PokemonNegocio();

            try
            {
                if(pokemon==null)
                {
                    pokemon= new Pokemon();
                }
                pokemon.Numero = int.Parse(tbNumero.Text);
                pokemon.Nombre = tbNombre.Text;
                pokemon.Descripcion = tbDescripcion.Text;
                pokemon.UrlImagen = tbUrl.Text;
                pokemon.Tipo = (Elemento)cboTipo.SelectedItem;
                pokemon.Debilidad = (Elemento)cboDebilidad.SelectedItem;

                if(pokemon.Id != 0)
                {
                    negocio.modificar(pokemon);
                    MessageBox.Show("Modificado exitosamente");
                }
                else
                {
                    negocio.agregar(pokemon);
                    MessageBox.Show("Agregado exitosamente");
                }

                //Guardo imagen si la levanto localmente
                if(archivo != null && !(tbUrl.Text.ToUpper().Contains("HTTP")))
                {
                    File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.FileName);
                }

                Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmAltaPokemon_Load(object sender, EventArgs e)
        {
            ElementoNegocio elementoNegocio= new ElementoNegocio();
            try
            {
                cboTipo.DataSource = elementoNegocio.listar();
                cboTipo.ValueMember = "Id";//Clave
                cboTipo.DisplayMember= "Descripcion";//Valor
                //ID y Descripcion son los nombres de las propiedades de la clase elemento, los uso como pares de clave/Valor, se eligen a gusto, puedo poner lo que quiera
                cboDebilidad.DataSource = elementoNegocio.listar();
                cboDebilidad.ValueMember= "Id";
                cboDebilidad.DisplayMember= "Descripcion";

                if(pokemon != null)
                {
                    tbNumero.Text = pokemon.Numero.ToString();
                    tbNombre.Text = pokemon.Nombre;
                    tbDescripcion.Text = pokemon.Descripcion;
                    tbUrl.Text = pokemon.UrlImagen;
                    cargarImagen(pokemon.UrlImagen);
                    cboTipo.SelectedValue = pokemon.Tipo.Id;
                    cboDebilidad.SelectedValue = pokemon.Debilidad.Id;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void tbUrl_Leave(object sender, EventArgs e)
        {
            cargarImagen(tbUrl.Text);
        }

        private void cargarImagen(string imagen)
        {
            try
            {
                pbPokemon.Load(imagen);
            }
            catch (Exception)
            {

                pbPokemon.Load("https://developers.elementor.com/docs/assets/img/elementor-placeholder-image.png");
            }
        }

        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg|png|*.png";
            if(archivo.ShowDialog() == DialogResult.OK)
            {
                tbUrl.Text = archivo.FileName;
                cargarImagen(archivo.FileName);
                //Guardo la imagen
                //File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.FileName);
            }
        }
    }
}
