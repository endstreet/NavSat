export interface Properties {
  Name: string;
  IsHealthy: boolean;
  Constellation: string;
}

export interface Geometry {
  type: string;
  coordinates: object;
}

export interface Feature {
  type: string;
  properties: Properties;
  geometry: Geometry;
}

export interface FeatureCollection {
  type: string;
  features: Feature[];
}
