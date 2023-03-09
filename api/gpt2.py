from transformers import pipeline, GPT2LMHeadModel, GPT2Tokenizer

tokenizer = GPT2Tokenizer.from_pretrained('gpt2')
model = GPT2LMHeadModel.from_pretrained('gpt2')

generator = pipeline('text-generation', model=model, tokenizer=tokenizer)

def generate_text(input : str, max_length : int = 100 ) -> str:
    output = generator(input, max_length=max_length, num_return_sequences=5)
    
    result = output[0]['generated_text']
    last_dot = result.rfind('.')
    return result[:(last_dot+1)]