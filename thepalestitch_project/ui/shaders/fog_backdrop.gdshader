shader_type canvas_item;
// Slow night fog for menu backgrounds. Put it on a full-screen ColorRect.

uniform vec3 deep : source_color = vec3(0.012, 0.014, 0.018);
uniform vec3 mist : source_color = vec3(0.2, 0.23, 0.23);
uniform float speed = 0.015;
uniform float vignette = 1.15;

float hash(vec2 p) {
	return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float value_noise(vec2 p) {
	vec2 i = floor(p);
	vec2 f = fract(p);
	vec2 u = f * f * (3.0 - 2.0 * f);
	return mix(mix(hash(i), hash(i + vec2(1.0, 0.0)), u.x),
			mix(hash(i + vec2(0.0, 1.0)), hash(i + vec2(1.0, 1.0)), u.x), u.y);
}

float fbm(vec2 p) {
	float total = 0.0;
	float amplitude = 0.5;
	for (int i = 0; i < 5; i++) {
		total += amplitude * value_noise(p);
		p = p * 2.03 + vec2(17.0, 9.0);
		amplitude *= 0.5;
	}
	return total;
}

void fragment() {
	vec2 uv = UV * vec2(3.2, 1.8);
	float drift = TIME * speed;
	float warp = fbm(uv + vec2(drift, drift * 0.3));
	float fog = fbm(uv * 1.3 + warp * 1.6 + vec2(-drift * 0.7, drift * 0.2));
	// Fog pools low on the screen, like on a street.
	float low = smoothstep(0.15, 1.0, UV.y);
	vec3 colour = mix(deep, mist, fog * fog * (0.25 + 0.75 * low));
	float edge = distance(UV, vec2(0.5)) * vignette;
	colour *= 1.0 - smoothstep(0.35, 0.9, edge);
	COLOR = vec4(colour, 1.0);
}
