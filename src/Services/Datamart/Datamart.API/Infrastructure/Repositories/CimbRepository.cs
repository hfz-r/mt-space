using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AHAM.Services.Datamart.API.Infrastructure.Exceptions;
using AHAM.Services.Datamart.API.Model;
using AHAM.Services.Datamart.API.Model.Envelopes;
using Dapper;
using Microsoft.Extensions.Logging;

namespace AHAM.Services.Datamart.API.Infrastructure.Repositories
{
    // todo: cache output
    public class CimbRepository : ICimbRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<CimbRepository> _logger;

        public CimbRepository(IDbConnection dbConnection, ILogger<CimbRepository> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public async Task<IList<FeeStructure>> GetAsync(CimbQuery query)
        {
            try
            {
                _logger.LogInformation("Fetching FeeStructure records");

                var filter = string.Empty;

                if (query.FeeType > 0) filter += "AND fs.FeeType = @FeeType";
                if (!string.IsNullOrEmpty(query.AgentId)) filter += "AND fs.AgentId = @AgentId";
                if (!string.IsNullOrEmpty(query.InvestorId)) filter += "AND fs.InvestorId = @InvestorId";
                if (!string.IsNullOrEmpty(query.ProductCode)) filter += "AND pr.[Code] = @ProductCode";

                var sql = $@"
SELECT TOP({query.Size}) fs.*, 
                 pr.Id, 
                 pr.Code, 
                 pr.Name, 
                 ac.Id, 
                 ac.Name, 
                 ag.Id, 
                 ag.Name
FROM FeeStructures AS fs
     LEFT JOIN Products AS pr ON fs.ProductId = pr.Id
     LEFT JOIN Accounts AS ac ON fs.InvestorId = ac.Id
     LEFT JOIN Agents AS ag ON fs.AgentId = ag.Id
WHERE pr.Id > 0
      AND ac.Id != ''
      AND ag.Id != ''
      {filter}
ORDER BY fs.ProductId, 
         fs.EffectiveFrom, 
         fs.InvestorId, 
         fs.AgentId
";

                var result = await _dbConnection.QueryAsync<FeeStructure, Product, Account, Agent, FeeStructure>(sql,
                    (fs, pr, ac, ag) =>
                    {
                        fs.Product = pr;
                        fs.Account = ac;
                        fs.Agent = ag;
                        return fs;
                    },
                    new {query.FeeType, query.AgentId, query.InvestorId, query.ProductCode},
                    splitOn: "*,Id,Id,Id"
                );

                return result.ToList();
            }
            catch (Exception e)
            {
                throw new DomainException("Failed to fetch records from database.", e);
            }
        }

        public async Task UpsertAsync(IEnumerable<CimbCommand> commands)
        {
            try
            {
                _logger.LogInformation("----- Creating/updating FeeStructure");

                var sql = commands.Select(cmd =>
                {
                    var feeStructure = _dbConnection.Query<FeeStructure>(
                        "SELECT TOP(1) * FROM FeeStructures WHERE FeeType = 70000 AND [Id] = @Id",
                        new {cmd.Id}).SingleOrDefault();

                    return feeStructure != null
                        ? cmd.Plan == "deleted"
                            ? $"DELETE FROM FeeStructures WHERE [Id] = {feeStructure.Id};"
                            : $@"
UPDATE fs
  SET 
      fs.EffectiveFrom = '{cmd.EffectiveFrom}',
      fs.InvestorId = '{cmd.InvestorId}',
      fs.AgentId = '{cmd.AgentId}'
FROM FeeStructures AS fs
WHERE fs.[Id] = {feeStructure.Id};
"
                        : $@"
INSERT INTO FeeStructures 
    SELECT '{cmd.EffectiveFrom}', [Id], '{cmd.AgentId}', '{cmd.InvestorId}', '', 'CS', 70000, 300, 0.00, 100000000000.00, 0.500000, NULL, NULL, 'AUM'
    FROM Products
    WHERE [Code] = '{cmd.ProductCode}';
";
                });

                var sqlJoin = string.Join("\n", sql);
                await _dbConnection.ExecuteAsync(sqlJoin);
            }
            catch (Exception e)
            {
                throw new DomainException("Failed to create and insert record into database.", e);
            }
        }
    }
}