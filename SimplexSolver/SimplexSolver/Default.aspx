<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SimplexSolver._Default" %>

<asp:Content ID="ScriptContent" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function addInput() {
            $("#restricoes").append("<div class='form-inline form-group my-3'><input type='text' class='form-control my-3 inputRestricao' aria-describedby='restricao' placeholder='Restrição do problema'><button class='btn btn-danger ml-1' onclick='removeRestricao(this)'>Remover</button></div>")
        }

    </script>

    <script type="text/javascript">
        function concatRestricoes() {
            const query = document.querySelectorAll('.inputRestricao');
            const values = [...query].map(x => x.value).join('@')
            console.log(values)
            document.getElementById('<%= hidenRestricoes.ClientID %>').value = values
        }
    </script>
    <script>
        function removeRestricao(element) {
            element.parentNode.remove();
        }
    </script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hidenRestricoes" Value="" runat="server" />

    <div class="jumbotron">
        <p class="lead">Trata-se de uma ferramenta online para resolução de problemas lineáres através do método Simplex. Para calcular, basta informar a função objetiva e suas restrições.</p>
        <strong>Considerações:</strong>
        <ul>
            <li>Variáveis devem ser representadas por uma letra</li>
            <li>Utilize sempre um espaçõ entre uma variável e um sinal de operação</li>
            <li>Não utilizar espaçõ entre uma variável e seu coeficiente</li>
        </ul>
    </div>
    <asp:Panel ID="pnlInfo" runat="server" CssClass="alert alert-danger" Visible="false" role="alert">
        <asp:Label ID="lblMsg" runat="server" Text="" />
    </asp:Panel>
    <div class="card card-body">
        <div class="row">
            <div class="col-md-4 col-sm-12 mt-3">
                <h2 class="text-center">Função objetiva</h2>
                <div class="alert alert-info alert-dismissible fade show" role="alert">
                    <strong>Padrão:</strong> Zmax = 1x + 2y
                </div>
                <div class="row">
                    <div class="col-md-12" id="eq">
                        <div class="form-inline form-group my-3">
                            <asp:DropDownList ID="cmbTipoFunc" CssClass="form-control " runat="server">
                                <asp:ListItem Text="Zmax" Value="0" />
                                <asp:ListItem Text="Zmin" Value="1" />
                            </asp:DropDownList><span class="mx-2">=</span>
                            <asp:TextBox CssClass="form-control" ID="txtEqBase" runat="server" placeholder="Função objetiva"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-4 col-sm-12 mt-3">
                <h2 class="text-center">Restrições</h2>
                <div class="alert alert-info alert-dismissible fade show" role="alert">
                    <strong>Padrão:</strong> 1x + 3y = 15
                </div>
                <div class="row">
                    <div class="col-md-12" id="restricoes">
                        <input type="text" class="form-control my-3 inputRestricao" aria-describedby="restricao" placeholder="Restrição do problema">
                    </div>
                </div>
                <button class="btn btn-sm btn-block btn-dark" type="button" onclick="addInput()">Adicionar</button>
            </div>
            <div class="col-md-4 col-sm-12 mt-3">
                <h2 class="text-center">Variáveis</h2>
                <div class="alert alert-info alert-dismissible fade show" role="alert">
                    <strong>Padrão:</strong> x > 0 ; y > 0
                </div>
                <div class="row">
                    <div class="col-md-12" id="var">
                        <asp:TextBox CssClass="form-control my-3" ID="txtVar" runat="server" placeholder="Variáveis"></asp:TextBox>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="text-right">
        <asp:Button CssClass="btn btn-dark" ID="btnResolverSimplex" runat="server" OnClientClick="concatRestricoes()" Text="Calcular resultado" OnClick="btnResolverSimplex_Click" />
    </div>
    <div class="my-3">
        <div class="card card-body">
            <asp:Label ID="lblResultados" runat="server" />
        </div>
    </div>


</asp:Content>
