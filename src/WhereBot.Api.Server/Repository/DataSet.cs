using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WhereBot.Api.Models;

namespace WhereBot.Api.Server
{

    public sealed class DataSet
    {

        #region Locking

        private readonly object lockObject = new object();

        public object LockObject
        {
            get
            {
                return this.lockObject;
            }
        }

        #endregion

        #region Storage

        public void ClearStorage()
        {
            lock (this.LockObject)
            {
                this.maps = new List<Map>();
                this.locations = new List<Location>();
                this.resources = new List<Resource>();
                this.nextMapId = 0;
                this.nextLocationId = 0;
                this.nextResourceId = 0;
            }
        }

        #endregion

        #region Maps

        private List<Map> maps = new List<Map>();
        private int nextMapId = 0;

        public IReadOnlyCollection<Map> GetMaps()
        {
            return this.maps.AsReadOnly();
        }

        public Map AddMap(string name, string description, string filename)
        {
            lock (this.LockObject)
            {
                try
                {
                    var map = this.AddMap(this.nextMapId, name, description, filename);
                    this.nextMapId += 1;
                    return map;
                }
                finally
                {
                    this.SaveMaps();
                }
            }
        }

        public Map AddMap(int id, string name, string description, string filename)
        {
            lock (this.LockObject)
            {
                try
                {
                    var current = this.maps.SingleOrDefault(m => (m.Id == id) || (m.Name == name));
                    if (current != null)
                    {
                        throw new InvalidOperationException("Map already exists.");
                    }
                    var map = new Map.Builder { Id = id, Name = name, Description = description, Filename = filename }.Build();
                    this.maps.Add(map);
                    return map;
                }
                finally
                {
                    this.SaveMaps();
                }
            }
        }

        private void SaveMaps()
        {
        }

        public void LoadMaps()
        {
            lock (this.LockObject)
            {
                var folder = "..\\..\\App_Data";
                var filename = Path.Combine(folder, "maps.json");
                using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var storage = (List<Storage.Map.Builder>)JsonSerializer.Create().Deserialize(reader, typeof(List<Storage.Map.Builder>));
                        this.maps = storage.Select(m => m.Build().ToModel()).ToList();
                    }
                }
                this.nextMapId = this.maps.Max(m => m.Id) + 1;
            }
        }

        #endregion

        #region Locations

        private List<Location> locations = new List<Location>();
        private int nextLocationId = 0;

        public IReadOnlyCollection<Location> GetLocations()
        {
            return this.locations.AsReadOnly();
        }

        public Location AddLocation(Map map, string name, int x, int y)
        {
            lock (this.LockObject)
            {
                try
                {
                    var location = this.AddLocation(this.nextLocationId, map, name, x, y);
                    this.nextLocationId += 1;
                    return location;
                }
                finally
                {
                    this.SaveLocations();
                }
            }
        }

        public Location AddLocation(int id, Map map, string name, int x, int y)
        {
            lock (this.LockObject)
            {
                try
                {
                    var current = this.locations.SingleOrDefault(l => (l.Id == id));
                    if (current != null)
                    {
                        throw new InvalidOperationException("Location already exists.");
                    }
                    var location = new Location.Builder { Id = id, Map = map, Name = name, X = x, Y = y }.Build();
                    this.locations.Add(location);
                    return location;
                }
                finally
                {
                    this.SaveLocations();
                }
            }
        }

        public void SaveLocations()
        {
            lock (this.LockObject)
            {
                var folder = "..\\..\\App_Data";
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                };
                var filename = Path.Combine(folder, "locations.json");
                using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        var storage = this.locations.Select(l => Storage.Location.FromModel(l)).ToList();
                        JsonSerializer.Create(settings).Serialize(writer, storage);
                    }
                }
            }
        }

        public void LoadLocations()
        {
            lock (this.LockObject)
            {
                var folder = "..\\..\\App_Data";
                var filename = Path.Combine(folder, "locations.json");
                using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var storage = (List<Storage.Location.Builder>)JsonSerializer.Create().Deserialize(reader, typeof(List<Storage.Location.Builder>));
                        this.locations = storage.Select(l => l.Build().ToModel(this)).ToList();
                    }
                }
                this.nextLocationId = this.locations.Max(l => l.Id) + 1;
            }
        }

        #endregion

        #region Resources

        private List<Resource> resources = new List<Resource>();
        private int nextResourceId = 0;

        public IReadOnlyCollection<Resource> GetResources()
        {
            return this.resources.AsReadOnly();
        }

        public Resource AddResource(string name)
        {
            return this.AddResource(name, null);
        }

        public Resource AddResource(string name, Location location)
        {
            lock (this.LockObject)
            {
                try
                {
                    var resource = this.AddResource(this.nextResourceId, name, location);
                    this.nextResourceId += 1;
                    return resource;
                }
                finally
                {
                    this.SaveResources();
                }
            }
        }

        public Resource AddResource(int id, string name, Location location)
        {
            lock (this.LockObject)
            {
                try
                {
                    var current = this.resources.SingleOrDefault(r => (r.Id == id) || (r.Name == name));
                    if (current != null)
                    {
                        throw new InvalidOperationException("Resource already exists.");
                    }
                    var resource = new Resource.Builder { Id = id, Name = name, Location = location }.Build();
                    this.resources.Add(resource);
                    return resource;
                }
                finally
                {
                    this.SaveResources();
                }
            }
        }

        public void RemoveResource(int id)
        {
            lock (this.LockObject)
            {
                try
                {
                    var current = this.resources.Single(r => (r.Id == id));
                    this.resources.Remove(current);
                }
                finally
                {
                    this.SaveResources();
                }
            }
        }

        public void SaveResources()
        {
            lock (this.LockObject)
            {
                var folder = "..\\..\\App_Data";
                var settings = new JsonSerializerSettings
                {
                    //Formatting = Formatting.Indented
                };
                var filename = Path.Combine(folder, "resources.json");
                using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        var storage = this.resources.Select(r => Storage.Resource.FromModel(r)).ToList();
                        JsonSerializer.Create(settings).Serialize(writer, storage);
                    }
                }
            }
        }

        public void LoadResources()
        {
            lock (this.LockObject)
            {
                var folder = "..\\..\\App_Data";
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                };
                var filename = Path.Combine(folder, "resources.json");
                using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var storage = (List<Storage.Resource.Builder>)JsonSerializer.Create(settings).Deserialize(reader, typeof(List<Storage.Resource.Builder>));
                        this.resources = storage.Select(r => r.Build().ToModel(this)).ToList();
                    }
                }
                this.nextResourceId = this.resources.Max(r => r.Id) + 1;
            }
        }

        #endregion

    }

}
