public class Avaliacao
{
    public int AvaliacaoId { get; set; }
    public int LivroId { get; set; }
    public Livro Livro { get; set; }
    public string UsuarioId { get; set; }
    public Usuario Usuario { get; set; }
    public int Nota { get; set; } // 0 a 5
    var livros = await _context.Livros
        .Select(l => new LivroCardViewModel
        {
            Livro = l,
            MediaAvaliacao = _context.Avaliacoes.Where(a => a.LivroId == l.LivroId).Average(a => (double?)a.Nota) ?? 0,
            QtdAvaliacoes = _context.Avaliacoes.Count(a => a.LivroId == l.LivroId)
        }).ToListAsync();
    public DateTime DataAvaliacao { get; set; }
    @model IEnumerable<LivroCardViewModel>
    @foreach (var card in Model)
    {
        < div class="card">
            <!-- ...capa, título, etc... -->
            <div>
                <span class="text-warning">
                    @for(int i = 1; i <= 5; i++)
                    {
                        <i class="bi @(i <= Math.Round(card.MediaAvaliacao) ? "bi-star-fill" : "bi-star")"></i>
                    }
                </ span >
                < span class= "ms-2" > (@card.MediaAvaliacao.ToString("0.0") / 5 - @card.QtdAvaliacoes avaliação(ões))</ span >
            </ div >
        </ div >
    }
    @model IEnumerable<AvaliacaoViewModel>
    @foreach (var item in Model)
    {
        < div ost]
    public async Task<IActionResult> Avaliar(int LivroId, int Nota)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var avaliacao = await _context.Avaliacoes
            .FirstOrDefaultAsync(a => a.LivroId == LivroId && a.UsuarioId == userId);

        if (avaliacao == null)
        {
            avaliacao = new Avaliacao
            {
                LivroId = LivroId,
                UsuarioId = userId,
                Nota = Nota,
                DataAvaliacao = DateTime.Now
            };
            _context.Avaliacoes.Add(avaliacao);
        }
        else
        {
            avaliacao.Nota = Nota;
            avaliacao.DataAvaliacao = DateTime.Now;
            _context.Avaliacoes.Update(avaliacao);
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
class="card mb-3">
            <div class="card-body">
                <h5>@item.Livro.Titulo</h5>
                <form asp-action="Avaliar" method="post">
                    <input type = "hidden" name="LivroId" value="@item.Livro.LivroId" />
                    <select name = "Nota" class="form-select w-auto d-inline">
                        @for(int i = 0; i <= 5; i++)
                        {
                            <option value = "@i" @(item.Nota == i? "selected" : "")>@i ?</option>
                        }
                    </ select >
                    < button type = "submit" class= "btn btn-primary ms-2" > Avaliar </ button >
                </ form >
            </ div >
        </ div >
    }
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Buscar livros que o usuário já retirou
        var livrosRetirados = await _context.Movimentacoes
            .Where(m => m.Usuario.AppUserId.ToString() == userId && m.DataRetirada != null)
            .Select(m => m.Livro)
            .Distinct()
            .ToListAsync();

        // Buscar avaliações já feitas pelo usuário
        var avaliacoes = await _context.Avaliacoes
            .Where(a => a.UsuarioId == userId)
            .ToListAsync();

        var viewModel = livrosRetirados.Select(livro => new AvaliacaoViewModel
        {
            Livro = livro,
            Nota = avaliacoes.FirstOrDefault(a => a.LivroId == livro.LivroId)?.Nota ?? 0
        }).ToList();

        return View(viewModel);
    }
}
