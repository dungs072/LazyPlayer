//! be cautious when using reference types

using System.Collections.Generic;

public struct GetStaffDataListQuery: IQueryResult<IReadOnlyList<CharacterData>> { }

public struct GetBuildingDataListQuery: IQueryResult<IReadOnlyList<BuildableEntity>> { }