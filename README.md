# GeoJSON

- https://geojson.org/

## MongoDB Help
- db.airports.ensureIndex({"loc":"2dsphere"});
    - Its necessary to filter radius by latitude, longitude and distance: construtor.NearSphere(x => x.loc, localization, distance)