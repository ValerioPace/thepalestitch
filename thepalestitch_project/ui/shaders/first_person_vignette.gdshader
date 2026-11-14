shader_type canvas_item;
// Darkens the edges of the screen while looking through the character's eyes,
// as if from deep inside a hood.

uniform float inner = 0.32;
uniform float outer = 0.78;
uniform float strength = 0.92;

void fragment() {
	vec2 centred = (UV - vec2(0.5)) * vec2(1.0, 1.25);
	float d = length(centred);
	COLOR = vec4(0.0, 0.0, 0.0, smoothstep(inner, outer, d) * strength);
}
