terraform {
  backend "s3" {
    bucket = "supergoon-terraform-plans"
    key    = "triple-triad"
    region = "us-east-2"
  }
}