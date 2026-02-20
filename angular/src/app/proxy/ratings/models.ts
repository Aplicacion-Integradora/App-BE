
export interface CreateRatingDto {
  seriesId: number;
  ratingNumber: number;
  comment?: string;
}

export interface RatingDto {
  userId?: string;
  id?: string;
  seriesId: number;
  ratingNumber: number;
  comment?: string;
  creationTime?: string;
}

export interface UpdateRatingDto {
  ratingNumber?: number;
  comment?: string;
}
