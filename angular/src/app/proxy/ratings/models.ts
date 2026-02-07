
export interface CreateRatingDto {
  seriesId?: string;
  rating: number;
  comment?: string;
}

export interface RatingDto {
  userId?: string;
  id?: string;
  seriesId?: string;
  rating: number;
  comment?: string;
  creationTime?: string;
}
