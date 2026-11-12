using MagasinGestion.Models;

namespace MagasinGestion.Services;

public sealed class PermissionService
{
    public void RequireAdministrator(Utilisateur? user) { if (user?.Role != RoleUtilisateur.Administrateur) throw new UnauthorizedAccessException("Droits administrateur requis."); }
}
