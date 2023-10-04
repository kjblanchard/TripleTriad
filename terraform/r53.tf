resource "aws_route53_record" "www" {
  for_each = toset(var.a_records)
  zone_id  = var.zone_id_for_record
  name     = "${each.value}.${var.domain_name}"
  type     = "A"
    alias {
    name                   = aws_cloudfront_distribution.triple_triad_distribution.domain_name
    zone_id                = aws_cloudfront_distribution.triple_triad_distribution.hosted_zone_id
    evaluate_target_health = true
  }
}
