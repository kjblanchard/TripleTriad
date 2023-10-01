# This cert is already created in my account
data "aws_acm_certificate" "supergoon_cert" {
  domain      = "*.supergoon.com"
  types       = ["AMAZON_ISSUED"]
  most_recent = true
}

locals {
  s3_origin = "s3-origin"
}

resource "aws_cloudfront_origin_access_control" "example" {
  name                              = "example"
  description                       = "Example Policy"
  origin_access_control_origin_type = "s3"
  signing_behavior                  = "always"
  signing_protocol                  = "sigv4"
}

# Create a CloudFront distribution pointing to the S3 bucket
resource "aws_cloudfront_distribution" "triple_triad_distribution" {
  origin {
    domain_name = aws_s3_bucket.triple_triad_bucket.bucket_regional_domain_name
    origin_access_control_id = aws_cloudfront_origin_access_control.example.id
    origin_id   = local.s3_origin
  }
  enabled             = true
  default_root_object = "index.html" # The main HTML file for your website
  aliases = ["tripletriad.supergoon.com"]

  restrictions {
    geo_restriction {
      restriction_type = "none"
    }
  }


  viewer_certificate {
    acm_certificate_arn = data.aws_acm_certificate.supergoon_cert.arn
    ssl_support_method  = "sni-only"
  }


  default_cache_behavior {
    allowed_methods  = ["DELETE", "GET", "HEAD", "OPTIONS", "PATCH", "POST", "PUT"]
    cached_methods   = ["GET", "HEAD"]
    target_origin_id = local.s3_origin

    forwarded_values {
      query_string = false

      cookies {
        forward = "none"
      }
    }

    viewer_protocol_policy = "allow-all"
    min_ttl                = 0
    default_ttl            = 3600
    max_ttl                = 86400
  }
  price_class = "PriceClass_100"
}
# Configure the S3 bucket to allow CloudFront to access it

resource "aws_s3_bucket_policy" "cloudfront_access_policy" {
  bucket = aws_s3_bucket.triple_triad_bucket.id
  policy = jsonencode({
    Version = "2012-10-17",
    Statement = [{
      Action = "s3:GetObject",
      Effect = "Allow",
      Resource = "${aws_s3_bucket.triple_triad_bucket.arn}/*",
      Principal = {
        Service = "cloudfront.amazonaws.com"
      }
      Condition = {
        StringEquals = {
            "AWS:SourceArn" = aws_cloudfront_distribution.triple_triad_distribution.arn
        }
      }
    }]
  })
}
# {
#     "Version": "2012-10-17",
#     "Statement": {
#         "Sid": "AllowCloudFrontServicePrincipalReadOnly",
#         "Effect": "Allow",
#         "Principal": {
#             "Service": "cloudfront.amazonaws.com"
#         },
#         "Action": "s3:GetObject",
#         "Resource": "arn:aws:s3:::<S3 bucket name>/*",
#         "Condition": {
#             "StringEquals": {
#                 "AWS:SourceArn": "arn:aws:cloudfront::<AWS account ID>:distribution/<CloudFront distribution ID>"
#             }
#         }
#     }
# }
# {
#     "Version": "2012-10-17",
#     "Statement": [
#         {
#             "Sid": "3",
#             "Effect": "Allow",
#             "Principal": {
#                 "AWS": "arn:aws:iam::cloudfront:user/CloudFront Origin Access Identity E2UAWQLWHHKE66"
#             },
#             "Action": "s3:GetObject",
#             "Resource": "arn:aws:s3:::kjb-unity-static-supergoon/*"
#         }
#     ]
# }
