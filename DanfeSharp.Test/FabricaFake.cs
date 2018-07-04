using DanfeSharp.Modelo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanfeSharp.Test
{
    public static class FabricaFake
    {
        public const double v = 1234.56;

        public static MemoryStream FakeLogo(int w, int h)
        {
            MemoryStream ms = new MemoryStream();
            using (Bitmap bmp = new Bitmap(w, h))            
            using (var graph = System.Drawing.Graphics.FromImage(bmp))
            {
                Rectangle ImageSize = new Rectangle(0, 0, w, h);
                graph.FillRectangle(Brushes.Cyan, ImageSize);
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            
            ms.Position = 0;
            return ms;
        }

        public static CalculoImpostoViewModel CalculoImpostoViewModel()
        {
            return new CalculoImpostoViewModel
            {
                BaseCalculoIcms = v,
                BaseCalculoIcmsSt = v,
                Desconto = v,
                OutrasDespesas = v,
                ValorAproximadoTributos = v,
                ValorCofins = v,
                ValorFrete = v,
                ValorIcms = v,
                ValorIcmsSt = v,
                ValorII = v,
                ValorIpi = v,
                ValorPis = v,
                ValorSeguro = v,
                ValorTotalNota = v,
                ValorTotalProdutos = v,
                vFCPUFDest = v,
                vICMSUFDest = v,
                vICMSUFRemet = v
            };
        }

        public static CalculoIssqnViewModel CalculoIssqnViewModel()
        {
            return new CalculoIssqnViewModel
            {
                BaseIssqn = v,
                InscricaoMunicipal = "123456789",
                Mostrar = true,
                ValorIssqn = v,
                ValorTotalServicos = v
            };
        }

        public static DanfeViewModel DanfeViewModel_1()
        {
            var m = new DanfeViewModel()
            {
                NfNumero = 888888888,
                NfSerie = 888,
                ChaveAcesso = new String('0', 44),
                Emitente = new EmpresaViewModel()
                {
                    CnpjCpf = new String('0', 14),
                    RazaoSocial = "Abstergo Ltda",
                    Email = "fake@mail.123",
                    EnderecoBairro = "Bairro",
                    EnderecoCep = "12345678",
                    EnderecoComplemento = "Compl",
                    EnderecoLogadrouro = "Avenida Brasil",
                    EnderecoNumero = "S/N",
                    EnderecoUf = "SP",
                    Municipio = "São Paulo",
                    Ie = "87787",
                    IeSt = "87878",
                    IM = "45454",
                    Telefone = "0000000000",
                    CRT = "3"
                },
                Destinatario = new EmpresaViewModel()
                {
                    CnpjCpf = new String('1', 14),
                    RazaoSocial = "Umbrella Corp Ltda",
                    Email = "fake@mail.123",
                    EnderecoBairro = "Bairro",
                    EnderecoCep = "12345678",
                    EnderecoComplemento = "Compl",
                    EnderecoLogadrouro = "Avenida Brasil",
                    EnderecoNumero = "S/N",
                    EnderecoUf = "SP",
                    Municipio = "São Paulo",
                    Ie = "87787",
                    IeSt = "87878",
                    IM = "45454",
                    Telefone = "0000000000"
                },
                Transportadora = new TransportadoraViewModel()
                {
                    RazaoSocial = "Correios",
                    CnpjCpf = new String('8', 14),
                    VeiculoUf = "RS",
                    QuantidadeVolumes = 123.1234,
                    CodigoAntt = new String('8', 20),
                    EnderecoBairro = "Bairo",
                    EnderecoCep = "00000",
                    EnderecoComplemento = "Complemento",
                    Especie = "Especie",
                    Placa = "MMMWWWW",
                    EnderecoLogadrouro = "Logadrouro",
                    Ie = "12334",
                    EnderecoUf = "RS",
                    PesoLiquido = 456.7794,
                    Marca = "DanfeSharp",
                    EnderecoNumero = "101",
                    ModalidadeFrete = 4,
                    PesoBruto = 101.1234
                },
                InformacoesComplementares = "Aqui vai as informações complementares."
            };
            m.CalculoImposto = CalculoImpostoViewModel();
            m.CalculoIssqn = CalculoIssqnViewModel();

            m.Duplicatas = new List<DuplicataViewModel>();

            for (int i = 1; i <= 10; i++)
            {
                var d = new DuplicataViewModel()
                {
                    Numero = i.ToString(),
                    Valor = i * Math.PI,
                    Vecimento = new DateTime(9999, 12, 30)
                };

                m.Duplicatas.Add(d);
            }

            m.Produtos = new List<ProdutoViewModel>();

            for (int i = 1; i <= 100; i++)
            {
                var p = new ProdutoViewModel()
                {
                    Descricao = $"Produto da linha {i}",
                    Codigo = i.ToString(),
                    Quantidade = i * Math.PI * 10,
                    AliquotaIcms = 99.88,
                    Unidade = "PEC",
                    Ncm = new String('8', 8)
                };

                if(i % 10 == 0)
                {
                    p.Descricao = string.Concat(Enumerable.Repeat(p.Descricao + " ", 15));
                }

                m.Produtos.Add(p);
            }


            return m;
        }



    }
}
