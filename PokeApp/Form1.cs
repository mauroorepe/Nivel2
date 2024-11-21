using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;

namespace PokeApp
{
    public partial class Form1 : Form
    {
        private List<Pokemon> listaPokemon;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cargar();
            cboCampo.Items.Add("Número");
            cboCampo.Items.Add("Nombre");
            cboCampo.Items.Add("Descripción");
        }

        private void dgvPokemons_SelectionChanged(object sender, EventArgs e)
        {
            //Utilizo casteo explicito (pokemon) porque c# me devuelve un object, trabaja con objects y yo le digo que es
            if(dgvPokemons.CurrentRow != null)
            {
                Pokemon seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.UrlImagen);
            }
           
        }

        private void cargar()
        {
            PokemonNegocio negocio = new PokemonNegocio();
            try
            {
                listaPokemon = negocio.listar();
                //DataSource recibe un origen de datos y los modela en la tabla
                dgvPokemons.DataSource = listaPokemon;
                ocultarColumnas();
                cargarImagen(listaPokemon[0].UrlImagen);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void ocultarColumnas()
        {
            dgvPokemons.Columns["UrlImagen"].Visible = false;
            dgvPokemons.Columns["Id"].Visible = false;
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

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAltaPokemon alta = new frmAltaPokemon();
            alta.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Pokemon seleccionado;
            seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;
            frmAltaPokemon modificar = new frmAltaPokemon(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        //La eliminacion fisica lanza un delete contra la base de datos y elimina permanentemente esos datos
        private void btnEliminarFisico_Click(object sender, EventArgs e)
        {
            eliminar();
        }

        //La eliminacion logica permite manejar un estado en el registro, que cuando el registro esta vivo ese estado esta activo y cuando el registro se elimina el estado pasa a estar inactivo, esta es la forma mas recomendable
        private void btnEliminacionLogica_Click(object sender, EventArgs e)
        {
            eliminar(true);
        }

        private void eliminar(bool logico = false) //si yo dejo el =false, hace que el parametro sea opcional, NO me lo va a pedir cuando invoque a la funcion y si no le digo nada lo toma como false, si no pusiera =false me pediria que le pase algo por parametro (Un Bool)
        {
            PokemonNegocio negocio = new PokemonNegocio();
            Pokemon seleccionado;
            try
            {
                DialogResult respuesta = MessageBox.Show("Estas seguro de eliminar el registro?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);//Mensaje modal cuando algo toma el control de la aplicacion y no puedo interactuar con nada mas que no sea eso, retorna un valor que yo puedo evaluar
                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;
                    if (logico)
                    {
                        negocio.eliminarLogico(seleccionado.Id);
                    }
                    else { 
                        negocio.eliminar(seleccionado.Id);
                    }
                    cargar();//Este metodo es para actualizar la grilla, para que yo no tenga que cerrar y volver a abrir el programa para que se actualice la grilla
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private bool validarFiltro()
        {
            if (cboCampo.SelectedIndex < 0) //Se comporta como colection, donde el indice va de 0 en adelante, si no hay NADA seleccionado, indice es -1
            {
                MessageBox.Show("Por favor seleccione el campo para filtrar.");
                return true;
            }
            if(cboCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor seleccione el criterio para filtrar.");
                return true;
            }
            if(cboCampo.SelectedItem.ToString() == "Número")
            {
                if (string.IsNullOrEmpty(tbFiltroAvanzado.Text))
                {
                    MessageBox.Show("Para buscar numeros debes escribir un numero.");
                    return true;
                }
                if (!(soloNumeros(tbFiltroAvanzado.Text)))
                {
                    MessageBox.Show("Por favor intrudzca solo numeros.");
                    return true;
                }
            }
            return false;
        }

        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                {
                    return false;
                }
            }
            return true;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            //List<Pokemon> listaFiltrada;
            //if (tbFiltro.Text != "")
            //{
            //    listaFiltrada = listaPokemon.FindAll(x => x.Nombre.ToUpper().Contains(tbFiltro.Text.ToUpper()) || x.Tipo.Descripcion.ToUpper().Contains(tbFiltro.Text.ToUpper()));//puedo usar .Contains para comparar coincidencias sin necesidad de una igualdad total y se le pasa como parametro el string que estoy comparando
            //}
            //else
            //{
            //    listaFiltrada = listaPokemon;
            //}

            //dgvPokemons.DataSource = null;
            //dgvPokemons.DataSource = listaFiltrada;
            //ocultarColumnas();
            PokemonNegocio negocio = new PokemonNegocio();
            try
            {
                if(validarFiltro())
                {
                    return;//corta la ejecucion, no retorna nada
                }
                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = tbFiltroAvanzado.Text;
                dgvPokemons.DataSource = negocio.filtrar(campo, criterio, filtro);

            }
            catch (Exception ex )
            {

                MessageBox.Show(ex.ToString());
            }

        }
        private void tbFiltro_TextChanged(object sender, EventArgs e)
        {
            List<Pokemon> listaFiltrada;
            if (tbFiltro.Text != "")//tbFiltro.Text.Lenght >= x num si quiero que filtre a partir de determinada cantidad de letras
            {
                listaFiltrada = listaPokemon.FindAll(x => x.Nombre.ToUpper().Contains(tbFiltro.Text.ToUpper()) || x.Tipo.Descripcion.ToUpper().Contains(tbFiltro.Text.ToUpper()));//puedo usar .Contains para comparar coincidencias sin necesidad de una igualdad total y se le pasa como parametro el string que estoy comparando
            }
            else
            {
                listaFiltrada = listaPokemon;
            }

            dgvPokemons.DataSource = null;
            dgvPokemons.DataSource = listaFiltrada;
            ocultarColumnas();
        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboCampo.SelectedItem.ToString();
            if (opcion == "Número")
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Mayor a");
                cboCriterio.Items.Add("Menor a");
                cboCriterio.Items.Add("Igual a");
            }
            else
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Comienza con");
                cboCriterio.Items.Add("Contiene");
                cboCriterio.Items.Add("Termina con");
            }
        }
    }
}

