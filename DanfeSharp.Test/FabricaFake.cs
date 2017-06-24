using DanfeSharp.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanfeSharp.Test
{
    public static class FabricaFake
    {
        public const double v = 1234.56;

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
                vFCP = v,
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
                    CnpjCpf = new String('0', 11),
                    Nome = "Abstergo Ltda",
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
                Destinatario = new EmpresaViewModel()
                {
                    CnpjCpf = new String('1', 11),
                    Nome = "Umbrella Corp Ltda",
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
                }
            };
            m.CalculoImposto = CalculoImpostoViewModel();
            m.CalculoIssqn = CalculoIssqnViewModel();

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
